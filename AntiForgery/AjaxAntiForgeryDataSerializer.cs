#region
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

#endregion

namespace bscheiman.Common.Aspnet.AntiForgery {
    internal class AjaxAntiForgeryDataSerializer {
        private IStateFormatter _formatter;

        protected internal IStateFormatter Formatter {
            get {
                if (_formatter == null)
                    _formatter = FormatterGenerator.GetFormatter();
                return _formatter;
            }
            set { _formatter = value; }
        }

        public virtual AjaxAntiForgeryData Deserialize(string serializedToken) {
            if (string.IsNullOrEmpty(serializedToken))
                throw new ArgumentException("Value cannot be null or empty.", "serializedToken");

            var formatter = Formatter;

            try {
                var deserializedObj = (object[]) formatter.Deserialize(serializedToken);
                return new AjaxAntiForgeryData {
                    Salt = (string) deserializedObj[0],
                    Value = (string) deserializedObj[1],
                    CreationDate = (DateTime) deserializedObj[2],
                    Username = (string) deserializedObj[3]
                };
            } catch (Exception ex) {
                throw CreateValidationException(ex);
            }
        }

        public virtual string Serialize(AjaxAntiForgeryData token) {
            if (token == null)
                throw new ArgumentNullException("token");

            object[] objToSerialize = {
                token.Salt, token.Value, token.CreationDate, token.Username
            };

            string serializedValue = Formatter.Serialize(objToSerialize);
            return serializedValue;
        }

        private static HttpAntiForgeryException CreateValidationException(Exception innerException) {
            return new HttpAntiForgeryException("Antiforgerytoken missing or validation failed.", innerException);
        }

        private static class FormatterGenerator {
            public static readonly Func<IStateFormatter> GetFormatter = TokenPersister.CreateFormatterGenerator();

            [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline",
                Justification = "This type must not be marked 'beforefieldinit'.")]
            static FormatterGenerator() {
            }

            private sealed class TokenPersister : PageStatePersister {
                private TokenPersister(Page page) : base(page) {
                }

                public static Func<IStateFormatter> CreateFormatterGenerator() {
                    // This code instantiates a page and tricks it into thinking that it's servicing
                    // a postback scenario with encrypted ViewState, which is required to make the
                    // StateFormatter properly decrypt data. Specifically, this code sets the
                    // internal Page.ContainsEncryptedViewState flag.
                    var writer = TextWriter.Null;
                    var response = new HttpResponse(writer);
                    var request = new HttpRequest("DummyFile.aspx", HttpContext.Current.Request.Url.ToString(),
                        "__EVENTTARGET=true&__VIEWSTATEENCRYPTED=true");
                    var context = new HttpContext(request, response);

                    var page = new Page {
                        EnableViewStateMac = true,
                        ViewStateEncryptionMode = ViewStateEncryptionMode.Always
                    };
                    page.ProcessRequest(context);

                    return () => new TokenPersister(page).StateFormatter;
                }

                public override void Load() {
                    throw new NotImplementedException();
                }

                public override void Save() {
                    throw new NotImplementedException();
                }
            }
        }
    }
}