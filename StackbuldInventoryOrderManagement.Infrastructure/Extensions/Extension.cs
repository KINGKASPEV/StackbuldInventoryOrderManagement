using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackbuldInventoryOrderManagement.Domain.Users;
using StackbuldInventoryOrderManagement.Persistence.Context;

namespace StackbuldInventoryOrderManagement.Persistence.Extensions
{
    public static class Extension
    {
        public static void AddDatabaseServices(
            this IServiceCollection services,
            IConfiguration configuration
        )
        {
            // Get the connection string from environment variables
            var connectionString = configuration["DATABASE_CONNECTION"];
            if (string.IsNullOrEmpty(connectionString))
                throw new ArgumentException(
                    "Database connection string is not configured in environment variables."
                );

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    dbOptions =>
                    {
                        dbOptions.EnableRetryOnFailure(3, TimeSpan.FromSeconds(10), null);
                        dbOptions.MigrationsAssembly("StackbuldInventoryOrderManagement.Persistence");
                    }
                );
            });

            services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    // User Signin/Lockout Settings
                    options.Lockout.MaxFailedAccessAttempts = 3;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                    options.User.AllowedUserNameCharacters =
                        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789_@";
                    options.User.RequireUniqueEmail = false;
                    // Password Settings
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 4;
                    options.Password.RequireLowercase = false;
                })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}
