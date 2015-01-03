#region
using System.Diagnostics;
using System.Web.Mvc;

#endregion

namespace bscheiman.Common.Aspnet.Extensions {
    public static class ActionExecutingContextExtensions {
        [DebuggerStepThrough]
        public static void Log(this ActionExecutingContext ctx) {
            if (ctx.HttpContext.Request.IsLocal)
                return;

            Util.Log.Info("Website hit from {0}: {1}/{2} ({3} - {4})", ctx.HttpContext.Request.UserHostAddress,
                ctx.RouteData.Values["controller"], ctx.RouteData.Values["action"], ctx.HttpContext.Request.HttpMethod,
                ctx.HttpContext.Request.UserAgent);
        }
    }
}