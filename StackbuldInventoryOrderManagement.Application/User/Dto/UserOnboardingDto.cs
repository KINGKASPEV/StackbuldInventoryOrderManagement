using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.User.Dto
{
    public record UserOnboardingDto(
        string FullName,
        string Email,
        string PhoneNumber,
        string Password,
        UserType UserType
    );
}
