#region
using System.Net.Http;
using System.Web.Http.Controllers;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class HttpControllerContextExtensions {
        public static void Log(this HttpControllerContext ctx, bool skipLocal = true) {
            if (skipLocal && ctx.Request.IsLocal())
                return;
        }
    }
}