namespace StackbuldInventoryOrderManagement.Application.User.Model
{
    public class LoginResponse
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? UserType { get; set; }
        public string? Role { get; set; } = String.Empty;
        public string? Token { get; set; }
        public bool FirstTimeLogin { get; set; }
        public string Email { get; set; }
    }

}
