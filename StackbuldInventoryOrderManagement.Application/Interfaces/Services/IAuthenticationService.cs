using StackbuldInventoryOrderManagement.Application.User.Dto;
using StackbuldInventoryOrderManagement.Application.User.Model;
using StackbuldInventoryOrderManagement.Common.Responses;

namespace StackbuldInventoryOrderManagement.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        Task<Response<LoginResponse>> LoginAsync(LoginRequestDto request);
        Task<Response<OnboardResponse>> SignUpAsync(UserOnboardingDto request);
    }
}
