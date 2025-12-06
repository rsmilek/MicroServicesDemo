using Msd.Services.AuthApi.Models;
using Msd.Services.AuthApi.Models.Dtos;
using Msd.Services.AuthApi.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Msd.Services.AuthApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = Role.Admin)]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// Add role to user
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="role">Role name</param>
        /// <returns>Operation result</returns>
        [HttpPost("add-role/{email}")]
        public async Task<ActionResult<ApiResponse<object>>> AddRoleToUser(string email, [FromQuery] string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ApiResponse<object>($"User {email} not found!"));

            if (!await _roleManager.RoleExistsAsync(role))
                return NotFound(new ApiResponse<object>($"Role = {role} doesn't exists!"));

            var result = await _userManager.AddToRoleAsync(user, role);            
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>()
                {
                    Message = $"User {email} successfully promoted to {role}.",
                    Data = new { email, role }
                });
            }

            return BadRequest(new ApiResponse<object>("Error adding role!") 
            { 
                Data = string.Join(',', result.Errors.Select(x => x.Description)) 
            });
        }

        /// <summary>
        /// Remove role from user
        /// </summary>
        /// <param name="email">User email</param>
        /// <param name="role">Role name</param>
        /// <returns>Operation result</returns>
        [HttpDelete("remove-role/{email}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveRoleFromUser(string email, [FromQuery] string role)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ApiResponse<object>($"User {email} not found!"));

            if (!await _roleManager.RoleExistsAsync(role))
                return NotFound(new ApiResponse<object>($"Role = {role} doesn't exists!"));

            var userRoles = await _userManager.GetRolesAsync(user);
            if (!userRoles.Contains(role))
                return BadRequest(new ApiResponse<object>($"User {email} doesn't have {role} role!"));
            if (userRoles.Count == 1)
                return BadRequest(new ApiResponse<object>($"Cannot remove {role} role from user {email} as it is the only role assigned!"));

            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>()
                {
                    Message = $"Role {role} successfully removed from user {email}.",
                    Data = new { email, role }
                });
            }

            return BadRequest(new ApiResponse<object>("Error removing role!")
            {
                Data = string.Join(',', result.Errors.Select(x => x.Description))
            });
        }

        /// <summary>
        /// Get list of all users
        /// </summary>
        /// <returns>List of users with their roles</returns>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<UserDto>>>> GetAllUsers()
        {
            var users = _userManager.Users.ToList();
            var userDtos = new List<UserDto>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(new UserDto
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return Ok(new ApiResponse<List<UserDto>>(userDtos));
        }

        /// <summary>
        /// Get list of all roles
        /// </summary>
        /// <returns>List of roles</returns>
        [HttpGet("roles")]
        public ActionResult<ApiResponse<List<string>>> GetAllRoles()
        {
            var roles = _roleManager.Roles
                .Select(r => r.Name)
                .Where(n => n != null)
                .Cast<string>()
                .ToList();

            return Ok(new ApiResponse<List<string>>(roles));
        }

        /// <summary>
        /// Delete user by email
        /// </summary>
        /// <param name="email">User email</param>
        /// <returns>Operation result</returns>
        [HttpDelete("delete-user/{email}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string email)
        {
            // Check if user exists
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound(new ApiResponse<object>($"User {email} not found!"));

            // Check if this is the last admin user
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(Role.Admin))
            {
                var adminUsers = await _userManager.GetUsersInRoleAsync(Role.Admin);
                if (adminUsers.Count == 1)
                    return BadRequest(new ApiResponse<object>($"Cannot delete user {email} as it is the only user with Admin role!"));
            }

            // Proceed to delete the user
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return Ok(new ApiResponse<object>()
                {
                    Message = $"User {email} successfully deleted.",
                    Data = new { email }
                });
            }

            return BadRequest(new ApiResponse<object>("Error deleting user!")
            {
                Data = string.Join(',', result.Errors.Select(x => x.Description))
            });
        }
    }
}
