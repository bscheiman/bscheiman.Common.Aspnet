#region
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using bscheiman.Common.Aspnet.Models;
using bscheiman.Common.Extensions;

#endregion

namespace bscheiman.Common.Aspnet.HtmlExtensions {
    public static class HtmlHelperExtensions {
        public static HtmlString ShowErrors(this HtmlHelper html, string[] errors) {
            if (errors == null || errors.Length == 0)
                return new HtmlString("");

            return new HtmlString(string.Format("<script>alert('{0}');</script>", string.Join("\n", errors).Replace("'", "\\'")));
        }

        public static HtmlString ShowErrors(this HtmlHelper html, BaseModel model) {
            return ShowErrors(html, model == null || model.Errors == null ? new string[0] : model.Errors);
        }

        public static HtmlString When(this HtmlHelper html, DateTime date, string format = "MMMM Do YYYY, h:mm:ss a") {
            if (date.Kind == DateTimeKind.Local)
                date = date.ToUniversalTime();

            Trace.WriteLine(date.ToString());
            Trace.WriteLine(date.ToEpoch());

            return new HtmlString(string.Format(@"<time datetime=""{0}"" data-format=""{1}"">", date.ToEpoch(), format));
        }

        public static HtmlString When(this HtmlHelper html, DateTime? date, string format = "MMMM Do YYYY, h:mm:ss a") {
            return When(html, date.Value, format);
        }
    }
}