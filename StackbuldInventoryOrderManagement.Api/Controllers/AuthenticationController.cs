using StackbuldInventoryOrderManagement.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
using StackbuldInventoryOrderManagement.Application.User.Dto;

namespace StackbuldInventoryOrderManagement.Api.Controllers
{
    /// <summary>
    /// Handles Authentication and Authorization operations.
    /// </summary>
    [Route("api/auth")]
    [ApiController]
    [Produces("application/json")]
    public class AuthenticationController(
        IAuthenticationService _authenticationService) : ControllerBase
    {

        /// <summary>
        /// Customer onboarding
        /// </summary>
        /// <param name="request">The Onboarding request containing full name, email, phone number and password.</param>
        [HttpPost("onboard")]
        public async Task<IActionResult> SignUp([FromBody] UserOnboardingDto request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _authenticationService.SignUpAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Login User
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (!ModelState.IsValid)
                return StatusCode(StatusCodes.Status400BadRequest, ModelState);

            var response = await _authenticationService.LoginAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}
