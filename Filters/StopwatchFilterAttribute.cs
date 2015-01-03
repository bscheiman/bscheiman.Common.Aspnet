#region
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

#endregion

namespace bscheiman.Common.Aspnet.Filters {
    public class StopwatchFilterAttribute : ActionFilterAttribute {
        public override Task OnActionExecutedAsync(HttpActionExecutedContext actionExecutedContext, CancellationToken cancellationToken) {
            var stopwatch = (Stopwatch) actionExecutedContext.Request.Properties["Stopwatch"];

            if (stopwatch != null) {
                actionExecutedContext.Response.Headers.Add("X-ExecutionTime",
                    (stopwatch.ElapsedMilliseconds / 1000.0f).ToString("N2", CultureInfo.InvariantCulture));
            }

            return base.OnActionExecutedAsync(actionExecutedContext, cancellationToken);
        }

        public override void OnActionExecuting(HttpActionContext context) {
            context.Request.Properties["Stopwatch"] = Stopwatch.StartNew();
        }
    }
}