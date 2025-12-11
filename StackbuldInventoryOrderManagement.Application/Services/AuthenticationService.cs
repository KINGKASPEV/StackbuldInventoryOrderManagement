using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using StackbuldInventoryOrderManagement.Application.User.Dto;
using StackbuldInventoryOrderManagement.Application.User.Model;
using StackbuldInventoryOrderManagement.Common.Config;
using StackbuldInventoryOrderManagement.Common.Responses;
using StackbuldInventoryOrderManagement.Common.Utilities;
using StackbuldInventoryOrderManagement.Domain.Users;

namespace StackbuldInventoryOrderManagement.Application.Services
{
    public class AuthenticationService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ILogger<AuthenticationService> logger,
        IOptions<AuthSettings> jwtData
    ) : IAuthenticationService
    {
        public async Task<Response<OnboardResponse>> SignUpAsync(UserOnboardingDto request)
        {
            var response = new Response<OnboardResponse>();

            try
            {
                if (request.UserType != UserType.Customer)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message =  Constants.OnlyCustomer;
                    return response;
                }

                var existingUser = await userManager.Users
                    .FirstOrDefaultAsync(x => x.Email == request.Email
                                           && x.UserType == UserType.Customer);

                if (existingUser != null)
                {
                    response.StatusCode = StatusCodes.Status409Conflict;
                    response.Message = Constants.UserDuplicateMessage;
                    return response;
                }

                var user = CreateCustomerEntity(request);

                var createResult = await userManager.CreateAsync(user, request.Password);
                if (!createResult.Succeeded)
                {
                    response.StatusCode = StatusCodes.Status400BadRequest;
                    response.Message = string.Join("; ", createResult.Errors.Select(e => e.Description));
                    return response;
                }

                var token = TokenService.GenerateUserToken(user, user.UserType.ToString(), jwtData);

                var mappedResponse = new OnboardResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber!,
                    UserType = user.UserType,
                    Email = user.Email!,
                    UserNumber = user.UserNumber,
                    Token = token
                };

                response.StatusCode = StatusCodes.Status201Created;
                response.Data = mappedResponse;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Sign up failed for phone {Phone}", request.PhoneNumber);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        public async Task<Response<LoginResponse>> LoginAsync(LoginRequestDto request)
        {
            var response = new Response<LoginResponse>();

            try
            {
                var user = await userManager.Users
                    .FirstOrDefaultAsync(x => x.Email == request.Email && x.UserType == request.UserType);

                if (user == null)
                {
                    response.StatusCode = StatusCodes.Status404NotFound;
                    response.Message = Constants.NotFoundMessage;
                    return response;
                }

                if (!user.IsActive)
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = Constants.AccountNotActive;
                    return response;
                }

                var result = await signInManager.PasswordSignInAsync(user, request.Password!, false, false);

                if (!result.Succeeded)
                {
                    response.StatusCode = StatusCodes.Status401Unauthorized;
                    response.Message = Constants.LoginFailure;
                    return response;
                }

                var token = TokenService.GenerateUserToken(user, user.UserType.ToString(), jwtData);

                var loginResponse = new LoginResponse
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    UserType = user.UserType.ToString(),
                    FirstTimeLogin = user.FirstTimeLogin,
                    Role = user.UserType.ToString(),
                    Token = token
                };

                response.StatusCode = StatusCodes.Status200OK;
                response.Data = loginResponse;
                response.Message = Constants.SuccessMessage;

                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Login failed for {Email}", request.Email);
                response.StatusCode = StatusCodes.Status500InternalServerError;
                response.Message = Constants.DefaultExceptionFriendlyMessage;
                return response;
            }
        }

        private ApplicationUser CreateCustomerEntity(UserOnboardingDto request)
        {
            string firstName = "";
            string lastName = "";

            if (!string.IsNullOrWhiteSpace(request.FullName))
            {
                var parts = request.FullName.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 1)
                {
                    firstName = parts[0];
                }
                else
                {
                    firstName = parts[0];
                    lastName = string.Join(" ", parts.Skip(1));
                }
            }

            return new ApplicationUser
            {
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                UserName = request.PhoneNumber,
                UserType = UserType.Customer,
                FirstTimeLogin = false,
                IsActive = true,
                DateCreated = DateTime.UtcNow
            };
        }
    }
}
