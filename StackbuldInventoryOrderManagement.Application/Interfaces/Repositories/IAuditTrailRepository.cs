namespace StackbuldInventoryOrderManagement.Application.Interfaces.Repositories
{
    public interface IAuditTrailRepository
    {
        void LogAction(
            string actionName,
            string actionDescription,
            string module,
            string loggedInUser,
            string ipAddress
        );
    }
}
