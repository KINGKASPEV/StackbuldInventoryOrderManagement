using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.User.Dto
{
    public class LoginRequestDto
    {
        public UserType UserType { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
    }
}
