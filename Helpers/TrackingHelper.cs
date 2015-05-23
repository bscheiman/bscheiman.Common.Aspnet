#region
using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using bscheiman.Common.Aspnet.Interfaces;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Helpers {
    internal static class TrackingHelper {
        internal static void TrackEntities(DbContext ctx) {
            var hasUsers =
                ctx.ChangeTracker.Entries()
                   .Where(x => x.Entity.Is<IHasUsers>() && (x.State == EntityState.Added || x.State == EntityState.Modified));
            var hasDates =
                ctx.ChangeTracker.Entries()
                   .Where(x => x.Entity.Is<IHasDates>() && (x.State == EntityState.Added || x.State == EntityState.Modified));
            var deleted = ctx.ChangeTracker.Entries().Where(x => x.Entity is ISoftDelete && x.State == EntityState.Deleted);

            string currentUsername = HttpContext.Current != null && HttpContext.Current.User != null
                ? HttpContext.Current.User.Identity.Name
                : "Anonymous";

            if (string.IsNullOrEmpty(currentUsername))
                currentUsername = "API";

            foreach (var entity in hasUsers) {
                var userObject = (IHasUsers) entity.Entity;

                if (entity.State == EntityState.Added)
                    userObject.UserCreated = currentUsername;

                userObject.UserModified = currentUsername;
            }

            foreach (var entity in hasDates) {
                var dateObject = (IHasDates) entity.Entity;

                if (entity.State == EntityState.Added)
                    dateObject.DateCreated = DateUtil.NowDt;

                dateObject.DateModified = DateUtil.NowDt;

                if (dateObject.DateCreated == DateTime.MinValue)
                    dateObject.DateCreated = DateUtil.NowDt;
            }

            foreach (var entity in deleted) {
                ((ISoftDelete) entity.Entity).IsDeleted = true;

                entity.State = EntityState.Modified;
            }
        }
    }
}