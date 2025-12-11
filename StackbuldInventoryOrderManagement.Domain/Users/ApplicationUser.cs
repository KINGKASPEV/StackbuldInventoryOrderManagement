using Microsoft.AspNetCore.Identity;

namespace StackbuldInventoryOrderManagement.Domain.Users
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string? UserNumber { get; set; }
        public string FirstName { get; set; } = String.Empty;
        public string? LastName { get; set; }
        public string? Address { get; set; }
        public UserType UserType { get; set; }
        public bool IsActive { get; set; } = true;
        public string? ImageUrl { get; set; }
        public string? Sex { get; set; } = String.Empty;
        public bool FirstTimeLogin { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now.ToUniversalTime();
        public DateTime? DateModified { get; set; }
        public override string UserName { get; set; } = String.Empty;
        public override string NormalizedUserName { get; set; } = String.Empty;
    }

}
