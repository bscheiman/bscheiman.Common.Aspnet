#region
using System;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.AntiForgery {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class AjaxValidateAntiForgeryTokenAttribute : FilterAttribute, IAuthorizationFilter {
        private string _salt;
        private AjaxAntiForgeryDataSerializer _serializer;

        public string Salt {
            get { return _salt ?? string.Empty; }
            set { _salt = value; }
        }

        internal AjaxAntiForgeryDataSerializer Serializer {
            get { return _serializer ?? (_serializer = new AjaxAntiForgeryDataSerializer()); }
            set { _serializer = value; }
        }

        public void OnAuthorization(AuthorizationContext filterContext) {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            string fieldName = AjaxAntiForgeryData.GetAntiForgeryTokenName(null);
            string cookieName = AjaxAntiForgeryData.GetAntiForgeryTokenName(filterContext.HttpContext.Request.ApplicationPath);

            var cookie = filterContext.HttpContext.Request.Cookies[cookieName];

            if (cookie == null || string.IsNullOrEmpty(cookie.Value))
                throw CreateValidationException();

            var cookieToken = Serializer.Deserialize(cookie.Value);

            string formValue = filterContext.HttpContext.Request.Headers[AjaxAntiForgeryData.GetAntiForgeryTokenHeaderName()];

            if (string.IsNullOrEmpty(formValue))
                throw CreateValidationException();

            var formToken = Serializer.Deserialize(formValue);

            if (!string.Equals(cookieToken.Value, formToken.Value, StringComparison.Ordinal))
                throw CreateValidationException();

            string currentUsername = AjaxAntiForgeryData.GetUsername(filterContext.HttpContext.User);

            if (!string.Equals(formToken.Username, currentUsername, StringComparison.OrdinalIgnoreCase))
                throw CreateValidationException();

            if (!ValidateFormToken(formToken))
                throw CreateValidationException();
        }

        private static HttpAntiForgeryException CreateValidationException() {
            return new HttpAntiForgeryException("A required anti-forgery token was not supplied or was invalid.");
        }

        private bool ValidateFormToken(AjaxAntiForgeryData token) {
            return (string.Equals(Salt, token.Salt, StringComparison.Ordinal));
        }
    }
}