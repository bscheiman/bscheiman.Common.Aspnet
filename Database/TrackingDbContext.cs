#region
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
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
            return TrackingHelper.SaveChanges(this);
        }

        public override Task<int> SaveChangesAsync() {
            return TrackingHelper.SaveChangesAsync(this);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
            return TrackingHelper.SaveChangesAsync(this, cancellationToken);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
        }
    }

    public class TrackingDbContext<TUser> : IdentityDbContext<TUser> where TUser : IdentityUser {
        public TrackingDbContext(string ctx) : base(ctx) {
        }

        public override int SaveChanges() {
            return TrackingHelper.SaveChanges(this);
        }

        public override Task<int> SaveChangesAsync() {
            return TrackingHelper.SaveChangesAsync(this);
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken) {
            return TrackingHelper.SaveChangesAsync(this, cancellationToken);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Conventions.Add(new ForeignKeyNamingConvention());
        }
    }
}