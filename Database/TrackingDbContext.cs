#region
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using bscheiman.Common.Aspnet.Helpers;
using Microsoft.AspNet.Identity.EntityFramework;

#endregion

namespace bscheiman.Common.Aspnet.Database {
    public class TrackingDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim> :
        IdentityDbContext<TUser, TRole, TKey, TUserLogin, TUserRole, TUserClaim>
        where TUser : IdentityUser<TKey, TUserLogin, TUserRole, TUserClaim>
        where TRole : IdentityRole<TKey, TUserRole>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserRole : IdentityUserRole<TKey>
        where TUserClaim : IdentityUserClaim<TKey> {
        public TrackingDbContext(string ctx) : base(ctx) {
        }

        public override int SaveChanges() {
            TrackingHelper.TrackEntities(this);

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync() {
            TrackingHelper.TrackEntities(this);

            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
            TrackingHelper.TrackEntities(this);

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
        }
    }

    public class TrackingDbContext : TrackingDbContext<IdentityUser> {
        public TrackingDbContext(string ctx) : base(ctx) {
        }
    }

    public class TrackingDbContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser {
        public TrackingDbContext(string ctx) : base(ctx) {
        }

        public override int SaveChanges() {
            TrackingHelper.TrackEntities(this);

            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync() {
            TrackingHelper.TrackEntities(this);

            return base.SaveChangesAsync();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
            TrackingHelper.TrackEntities(this);

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
        }
    }
}