using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using StackbuldInventoryOrderManagement.Application.Interfaces.Persistence;
using StackbuldInventoryOrderManagement.Domain.Audit;
using StackbuldInventoryOrderManagement.Domain.Common;
using StackbuldInventoryOrderManagement.Domain.Orders;
using StackbuldInventoryOrderManagement.Domain.Products;
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
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public override async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = new CancellationToken()
        )
        {
            foreach (EntityEntry<Entity> entity in ChangeTracker.Entries<Entity>())
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.Entity.DateCreated = DateTime.UtcNow;
                        break;
                    case EntityState.Modified:
                        entity.Entity.DateModified = DateTime.UtcNow;
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

            builder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
                entity.Property(e => e.Price).HasPrecision(18, 2);
                entity.Property(e => e.Sku).HasMaxLength(50);
                entity.HasIndex(e => e.Sku).IsUnique();
                entity.HasIndex(e => e.Name);
            });

            builder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.OrderNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);
                entity.HasIndex(e => e.OrderNumber).IsUnique();
                entity.HasIndex(e => e.CustomerId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.DateCreated);

                entity.HasOne(e => e.Customer)
                    .WithMany()
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<OrderItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UnitPrice).HasPrecision(18, 2);
                entity.Property(e => e.TotalPrice).HasPrecision(18, 2);

                entity.HasOne(e => e.Order)
                    .WithMany(o => o.OrderItems)
                    .HasForeignKey(e => e.OrderId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.OrderItems)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

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


