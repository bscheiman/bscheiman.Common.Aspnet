#region
using System.Data.Entity;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using bscheiman.Common.Aspnet.Extensions;

#endregion

namespace bscheiman.Common.Aspnet.Controllers {
    public class ApiLoggingController<TDatabase> : ApiController where TDatabase : DbContext, new() {
        protected TDatabase Database { get; set; }

        public ApiLoggingController(TDatabase database) {
            Database = database;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext ctx, CancellationToken cancellationToken) {
            ctx.Log();

            return base.ExecuteAsync(ctx, cancellationToken);
        }
    }
}