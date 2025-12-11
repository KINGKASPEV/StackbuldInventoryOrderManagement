using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.User.Model
{
    public class OnboardResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string? LastName { get; set; }
        public string PhoneNumber { get; set; }
        public UserType UserType { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string? UserNumber { get; set; }
    }
}
