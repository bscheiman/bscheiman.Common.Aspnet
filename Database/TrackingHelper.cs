#region
using System.Data.Entity;
using System.Linq;
using System.Web;
using bscheiman.Common.Aspnet.Interfaces;
using bscheiman.Common.Extensions;
using bscheiman.Common.Util;

#endregion

namespace bscheiman.Common.Aspnet.Database {
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
                if (entity.State == EntityState.Added)
                    ((IHasUsers) entity.Entity).UserCreated = currentUsername;

                ((IHasUsers) entity.Entity).UserModified = currentUsername;
            }

            foreach (var entity in hasDates) {
                if (entity.State == EntityState.Added)
                    ((IHasDates) entity.Entity).DateCreated = DateUtil.NowDt;

                ((IHasDates) entity.Entity).DateModified = DateUtil.NowDt;
            }

            foreach (var entity in deleted) {
                ((ISoftDelete) entity.Entity).IsDeleted = true;

                entity.State = EntityState.Modified;
            }
        }
    }
}