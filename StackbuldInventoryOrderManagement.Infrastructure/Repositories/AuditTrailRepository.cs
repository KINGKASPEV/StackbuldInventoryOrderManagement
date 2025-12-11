using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using StackbuldInventoryOrderManagement.Application.Interfaces.Repositories;
using StackbuldInventoryOrderManagement.Domain.Audit;

namespace StackbuldInventoryOrderManagement.Persistence.Repositories
{
    public class AuditTrailRepository(IConfiguration configuration) : IAuditTrailRepository
    {
        public async void LogAction(
            string actionName,
            string actionDescription,
            string module,
            string loggedInUser,
            string ipAddress
        )
        {
            var sql =
                $"INSERT INTO \"{configuration["db.schema"]}\".\"AuditTrail\" "
                + $"(\"ActionName\", \"ActionDescription\", \"Module\", \"LoggedInUser\", \"Origin\", \"DateCreated\", \"ActionTime\", \"CreatedBy\") "
                + $"VALUES "
                + $"(@ActionName, @ActionDescription, @Module, @LoggedInUser, @Origin, @DateCreated, @ActionTime, @CreatedBy)";

            var auditTrailObject = new AuditTrail()
            {
                ActionName = actionName ?? module,
                ActionDescription = actionDescription,
                Module = module,
                LoggedInUser = loggedInUser,
                ActionTime = DateTime.UtcNow,
                Origin = ipAddress,
                DateCreated = DateTime.UtcNow,
                CreatedBy = loggedInUser,
            };

            //Retrieve connection string from environment variable
            var connectionString = configuration["ERANDE_CONNECTION"];
            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException(
                    "Database connection string is not configured in the environment variables."
                );

            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
            await connection.ExecuteAsync(sql, auditTrailObject);
            await connection.CloseAsync();
        }
    }
}
