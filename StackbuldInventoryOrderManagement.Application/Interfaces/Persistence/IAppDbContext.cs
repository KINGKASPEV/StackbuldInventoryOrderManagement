using Microsoft.EntityFrameworkCore;
using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.Interfaces.Persistence
{
    public interface IAppDbContext
    {
        DbSet<ApplicationUser?> Users { get; set; }
    }
}
