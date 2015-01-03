#region
using System.Web;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.AntiForgery {
    public static class AjaxAntiForgeryHtmlExtensions {
        private static AjaxAntiForgeryDataSerializer _serializer;

        internal static AjaxAntiForgeryDataSerializer Serializer {
            get { return _serializer ?? (_serializer = new AjaxAntiForgeryDataSerializer()); }
            set { _serializer = value; }
        }

        public static MvcHtmlString AjaxAntiForgeryToken(this HtmlHelper helper) {
            return AjaxAntiForgeryToken(helper, null /* salt */);
        }

        public static MvcHtmlString AjaxAntiForgeryToken(this HtmlHelper helper, string salt) {
            return AjaxAntiForgeryToken(helper, salt, null /* domain */, null /* path */);
        }

        public static MvcHtmlString AjaxAntiForgeryToken(this HtmlHelper helper, string salt, string domain, string path) {
            string formValue = GetAntiForgeryTokenAndSetCookie(helper, salt, domain, path);
            string fieldName = AjaxAntiForgeryData.GetAntiForgeryTokenName(null);

            var builder = new TagBuilder("meta");
            builder.Attributes["name"] = fieldName;
            builder.Attributes["content"] = formValue;

            return MvcHtmlString.Create(builder.ToString(TagRenderMode.StartTag));
        }

        private static string GetAntiForgeryTokenAndSetCookie(this HtmlHelper helper, string salt, string domain, string path) {
            string cookieName = AjaxAntiForgeryData.GetAntiForgeryTokenName(helper.ViewContext.HttpContext.Request.ApplicationPath);

            AjaxAntiForgeryData cookieToken;
            var cookie = helper.ViewContext.HttpContext.Request.Cookies[cookieName];

            if (cookie != null)
                cookieToken = Serializer.Deserialize(cookie.Value);
            else {
                cookieToken = AjaxAntiForgeryData.NewToken();
                string cookieValue = Serializer.Serialize(cookieToken);

                var newCookie = new HttpCookie(cookieName, cookieValue) {
                    HttpOnly = true,
                    Domain = domain
                };
                if (!string.IsNullOrEmpty(path))
                    newCookie.Path = path;
                helper.ViewContext.HttpContext.Response.Cookies.Set(newCookie);
            }

            var formToken = new AjaxAntiForgeryData(cookieToken) {
                Salt = salt,
                Username = AjaxAntiForgeryData.GetUsername(helper.ViewContext.HttpContext.User)
            };

            string formValue = Serializer.Serialize(formToken);

            return formValue;
        }
    }
}