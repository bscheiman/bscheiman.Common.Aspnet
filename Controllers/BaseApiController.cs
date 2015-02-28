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
        private bool DatabaseDisposed { get; set; }

        public ApiLoggingController() {
            Database = new TDatabase();
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext ctx, CancellationToken cancellationToken) {
            ctx.Log();

            return base.ExecuteAsync(ctx, cancellationToken);
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);

            Cleanup();
        }

        protected TDatabase GetContext() {
            return Database;
        }

        private void Cleanup() {
            if (DatabaseDisposed || Database == null)
                return;

            Database.SaveChanges();
            Database.Dispose();

            DatabaseDisposed = true;
        }
    }
}