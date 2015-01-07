#region
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using bscheiman.Common.Aspnet.Extensions;
using bscheiman.Common.Aspnet.Interfaces;
using bscheiman.Common.Aspnet.Objects;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Filters {
    public abstract class HeaderValidationFilterAttribute : ActionFilterAttribute {
        public override void OnActionExecuting(HttpActionContext context) {
            string apiKey = context.Request.Headers.Get("X-ApiKey");
            string hash = context.Request.Headers.Get("X-Hash");
            long ts = context.Request.Headers.Get("X-Timestamp").ToInt64();

            if (apiKey.IsNullOrEmpty() || hash.IsNullOrEmpty() || ts == 0) {
                context.Response = CreateJson(403, "please specify ApiKey/Hash/Timestamp");

                return;
            }

            if ((DateUtil.Now - ts) > 300) {
                context.Response = CreateJson(403, "too late, bro");

                return;
            }

            var apiUser = GetApiUser(apiKey);

            if (apiUser == null)
                context.Response = CreateJson(403, "invalid api user");
            else {
                string str = string.Format("{0}:{1}:{2}", apiKey, context.Request.RequestUri, ts);
                string computedHash = str.ToHMAC256(apiUser.ApiSecret);

                if (computedHash != hash.ToUpper())
                    context.Response = CreateJson(403, "invalid hash");
            }
        }

        protected abstract IHasApiKeys GetApiUser(string apiKey);

        private HttpResponseMessage CreateJson(int status, string error) {
            return new HttpResponseMessage {
                Content = new StringContent(new BaseJson {
                    Code = status,
                    Valid = false,
                    Message = error
                }.ToJson())
            };
        }
    }
}