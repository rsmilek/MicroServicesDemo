using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Msd.Services.AuthApi.Models;
using Msd.Services.AuthApi.Models.Dtos;
using Msd.Services.AuthApi.Services;
using System.Security.Claims;
using System.Text.Json;

namespace Msd.Services.AuthApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet("signin/microsoft")]
        [AllowAnonymous]
        public async Task SigninMicrosoft(string redirectUrl)
        {
            var result = await Request.HttpContext.AuthenticateAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            if (!result.Succeeded
                || result?.Principal == null
                || !result.Principal.Identities.Any(id => id.IsAuthenticated)
                || string.IsNullOrEmpty(result.Properties.GetTokenValue("access_token")))
            {
                // Not authenticated -> challenge
                await Request.HttpContext.ChallengeAsync(MicrosoftAccountDefaults.AuthenticationScheme);
            }
            else
            {
                var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
                var email = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                var name = claims?.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
                var picture = claims?.FirstOrDefault(c => c.Type == "picture")?.Value;

                // Claims email verification
                if (string.IsNullOrEmpty(email))
                {
                    Response.StatusCode = 400;
                    Response.ContentType = "application/json";
                    await Response.Body.WriteAsync(JsonSerializer.SerializeToUtf8Bytes(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Missing email address – cannot create account."
                    }));
                    return;
                }

                // User verification in database
                var user = await _authService.CreateUserWithRoleAsync(email, socialSign: true);

                // Sign in the user
                var loginResponseDto = await _authService.SignInAsync(user, name, picture);

                // Redirect to final url
                var url = $"{redirectUrl}?token={loginResponseDto.Token}";
                Request.HttpContext.Response.Redirect(url);
            }
        }

        [HttpPost("signup")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> SignUp([FromBody] RegistrationRequestDto registerRequestDto)
        {
            try
            {
                var loginResponseDto = await _authService.RegisterAsync(registerRequestDto);
                return Ok(new ApiResponse<LoginResponseDto>(loginResponseDto));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<LoginResponseDto>(ex.Message));
            }
        }

        [HttpPost("signin/email")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> SignInEmail([FromBody] LoginRequestDto loginRequestDto)
        {
            try
            {
                var loginResponseDto = await _authService.LoginAsync(loginRequestDto);
                return Ok(new ApiResponse<LoginResponseDto>(loginResponseDto));
            }
            catch (Exception ex)
            {
                return Ok(new ApiResponse<LoginResponseDto>(ex.Message));
            }
        }

    }
}

