#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;
using Postal;
using RestSharp;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class SendMail {
        private static string Account { get; set; }
        private static string Domain { get; set; }
        private static string Password { get; set; }

        public static void Init(string account, string password, string domain) {
            Account = account;
            Password = password;
            Domain = domain;
        }

        public static async Task<bool> To(MailMessage message) {
            if (Account.IsNullOrEmpty() || Password.IsNullOrEmpty() || Domain.IsNullOrEmpty()) {
                Log.Fatal("Can't send mail - invalid config");

                return false;
            }

            try {
                var client = new RestClient {
                    BaseUrl = new Uri("https://api.mailgun.net/v2"),
                    Authenticator = new HttpBasicAuthenticator(Account, Password)
                };
                var request = new RestRequest();
                request.AddParameter("domain", Domain, ParameterType.UrlSegment);
                request.Resource = "{domain}/messages";
                request.AddParameter("from", string.Format("{0} <{1}>", message.From.DisplayName, message.From.Address));

                foreach (var to in message.To)
                    request.AddParameter("to", to.Address);

                request.AddParameter("subject", message.Subject);
                request.AddParameter("html", message.Body);

                var plainText = message.AlternateViews.FirstOrDefault(c => c.ContentType.MediaType == "text/plain");

                if (plainText != null)
                    request.AddParameter("text", plainText);

                foreach (var attachment in message.Attachments) {
                    using (var ms = new MemoryStream()) {
                        await attachment.ContentStream.CopyToAsync(ms);

                        request.AddFile("attachment", ms.ToArray(), attachment.Name);
                    }
                }

                request.Method = Method.POST;

                return client.Execute(request).ResponseStatus == ResponseStatus.Completed;
            } catch (Exception ex) {
                Log.Error("SendMail: {0}", ex.ToString());

                return false;
            }
        }

        public static Task<bool> To<TMail>(string address, string viewName, TMail model) where TMail : Email {
            model.ViewName = viewName;
            var msg = new EmailService().CreateMailMessage(model);
            msg.To.Add(new MailAddress(address));

            return To(msg);
        }

        public static Task<bool> To(string address, string subject, string html, string plainText, string from, string fromName,
                                    IEnumerable<HttpPostedFileBase> files = null) {
            var message = new MailMessage(new MailAddress(from, fromName), new MailAddress(address)) {
                Subject = subject,
                Body = html
            };

            message.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, new ContentType("text/plain")));

            if (files != null) {
                foreach (var file in files.ToArray())
                    message.Attachments.Add(new Attachment(file.InputStream, file.FileName));
            }

            return To(message);
        }
    }
}