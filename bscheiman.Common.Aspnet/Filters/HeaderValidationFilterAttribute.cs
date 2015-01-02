#region
using System.Data.Entity;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using bscheiman.Common.Aspnet.HtmlExtensions;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Filters {
    public class HeaderValidationFilterAttribute<T> : ActionFilterAttribute where T : DbContext, new() {
        public override void OnActionExecuting(HttpActionContext context) {
            using (var database = new T()) {
                string apiKey = context.Request.Headers.Get("X-ApiKey");
                string hash = context.Request.Headers.Get("X-Hash");
                long ts = context.Request.Headers.Get("X-Timestamp").ToInt64();

                if (string.IsNullOrEmpty(apiKey) || string.IsNullOrEmpty(hash) || ts == 0) {
                    context.Response = CreateJson(403, "please specify ApiKey/Hash/Timestamp");

                    return;
                }

                if ((DateUtil.Now - ts) > 300) {
                    context.Response = CreateJson(403, "too late, bro");

                    return;
                }

                var business = database.Businesses.FirstOrDefault(b => b.ApiKey == apiKey);

                if (business == null) {
                    context.Response = CreateJson(403, "invalid api user");

                    return;
                }

                string str = string.Format("{0}:{1}:{2}", apiKey, context.Request.RequestUri, ts);
                string computedHash = str.ToHMAC256(business.ApiSecret);

                if (computedHash != hash.ToUpper())
                    context.Response = CreateJson(403, "invalid hash");

                // CacheUtil.Set(business);
            }
        }

        private HttpResponseMessage CreateJson(int status, string error) {
            return new HttpResponseMessage {
                Content = new StringContent(new {
                    status,
                    error
                }.ToJson())
            };
        }
    }
}