﻿#region
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

        public ApiLoggingController() {
            Database = new TDatabase();
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext ctx, CancellationToken cancellationToken) {
            ctx.Log();

            return base.ExecuteAsync(ctx, cancellationToken);
        }

        protected TDatabase GetContext() {
            return Database;
        }

        protected override void Dispose(bool disposing) {
            base.Dispose(disposing);
            
            Database.SaveChanges();
            Database.Dispose();
        }
    }
}