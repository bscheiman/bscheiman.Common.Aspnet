#region
using System;
using System.Collections.Generic;
using System.Web;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;
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

        public static bool To(string address, string subject, string html, string plaintext, string from, string fromName,
            IEnumerable<HttpPostedFileBase> files = null) {
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
                request.AddParameter("from", string.Format("{0} <{1}>", fromName, from));
                request.AddParameter("to", address);
                request.AddParameter("subject", subject);
                request.AddParameter("html", html);
                request.AddParameter("text", plaintext);

                if (files != null) {
                    foreach (var f in files) {
                        var b = new byte[f.InputStream.Length];

                        f.InputStream.Position = 0;
                        f.InputStream.Read(b, 0, b.Length);

                        request.AddFile("attachment", b, f.FileName);
                    }
                }

                request.Method = Method.POST;

                return client.Execute(request).ResponseStatus == ResponseStatus.Completed;
            } catch (Exception ex) {
                Log.Error("SendMail: {0}", ex.ToString());

                return false;
            }
        }
    }
}