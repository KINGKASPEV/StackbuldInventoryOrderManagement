using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StackbuldInventoryOrderManagement.Application.Interfaces.Persistence;
using StackbuldInventoryOrderManagement.Domain.Audit;
using StackbuldInventoryOrderManagement.Domain.Common;
using StackbuldInventoryOrderManagement.Domain.Users;
using System.Reflection;

namespace StackbuldInventoryOrderManagement.Persistence.Context
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>, IAppDbContext
    {
        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public static AppDbContext Create()
        {
            return new AppDbContext();
        }

        public DbSet<AuditTrail> AuditTrail { get; set; }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            foreach (EntityEntry<Entity> entity in ChangeTracker.Entries<Entity>())
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.Entity.DateCreated = DateTime.Now.ToUniversalTime();
                        break;
                    case EntityState.Modified:
                        entity.Entity.DateModified = DateTime.Now.ToUniversalTime();
                        break;
                }
            }

            var result = await base.SaveChangesAsync(cancellationToken);
            return result;
        }

        public async Task<bool> SaveAllChangeAsync(CancellationToken cancellationToken)
        {
            if (await SaveChangesAsync(cancellationToken) > 0)
                return true;
            else
                return false;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            builder.Entity<AuditTrail>().HasNoKey();

            this.SeedCompany(builder);
        }

        public void SeedCompany(ModelBuilder builder)
        {
            string email = "superadmin@kingsley.com";
            var adminUserId = Guid.NewGuid().ToString();
            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                FirstName = "Kingsley",
                LastName = "Okafor",
                Email = email,
                DateCreated = DateTime.UtcNow,
                NormalizedEmail = email.ToUpper(),
                UserName = email,
                NormalizedUserName = email.ToUpper(),
                Address = "Lagos",
                UserType = UserType.Admin,
            };

            PasswordHasher<ApplicationUser> hasher = new PasswordHasher<ApplicationUser>();
            var hash = hasher.HashPassword(adminUser, "P@ssw0rd");
            adminUser.PasswordHash = hash;
            builder.Entity<ApplicationUser>().HasData(adminUser);
        }
    }
}
