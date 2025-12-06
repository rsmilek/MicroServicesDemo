using Msd.Services.AuthApi.Models;
using Msd.Services.AuthApi.Models.Dtos;
using Msd.Services.AuthApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Msd.Services.AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Get information about the currently logged-in user
        /// </summary>
        /// <returns>User information</returns>
        [HttpGet("me")]
        [Authorize(Roles = $"{Role.User},{Role.Admin}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetCurrentUser()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (email == null)
                return Unauthorized(new ApiResponse<UserDto>("Invalid token"));

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized(new ApiResponse<UserDto>("User not found"));

            var name = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? user.UserName;
            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new ApiResponse<UserDto>
            {
                Data = new UserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Name = name,
                    Roles = [.. roles]
                }
            });
        }

    }
}

