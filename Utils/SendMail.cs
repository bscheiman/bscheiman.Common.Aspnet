#region
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Web;
using bscheiman.Common.Aspnet.Helpers;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;
using RestSharp;

#endregion

namespace bscheiman.Common.Aspnet.Utils {
    public static class SendMail {
        private static string Account { get; set; }
        private static string Domain { get; set; }
        public static bool Initialized { get; set; }
        private static string Password { get; set; }

        public static void Init(string account, string password, string domain) {
            account.ThrowIfNullOrEmpty("account");
            password.ThrowIfNullOrEmpty("password");
            domain.ThrowIfNullOrEmpty("domain");

            Account = account;
            Password = password;
            Domain = domain;

            Initialized = true;
        }

        public static async Task<bool> To(MailMessage message) {
            if (!Initialized) {
                Log.Fatal("Can't send mail - invalid config");

                return false;
            }

            try {
                var client = new RestClient {
                    BaseUrl = new Uri("https://api.mailgun.net/v3"),
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

        public static async Task<bool> To(string address, string from, string fromName, string subject, string viewName, object model) {
            if (!viewName.StartsWith("~"))
                viewName = "~/Views/Emails/" + viewName;

            if (!viewName.EndsWith(".md"))
                viewName += ".md";

            string final = MarkdownHelper.Transform(RazorHelper.Transform(PathHelper.MapRelative(viewName), model),
                PathHelper.ReadAsString("~/markdown-email.css"), true);

            return await To(address, subject, final, "", from, fromName);
        }

        public static Task<bool> To(string address, string subject, string html, string plainText, string from, string fromName,
                                    IEnumerable<HttpPostedFileBase> files = null) {
            var message = new MailMessage(new MailAddress(from, fromName), new MailAddress(address)) {
                Subject = subject,
                Body = html,
                IsBodyHtml = true
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