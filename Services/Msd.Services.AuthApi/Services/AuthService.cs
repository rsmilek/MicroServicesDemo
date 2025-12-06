using Msd.Services.AuthApi.Models.Dtos;
using Msd.Services.AuthApi.Utility;
using Microsoft.AspNetCore.Identity;

namespace Msd.Services.AuthApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService;

        public AuthService(
            ITokenService tokenService,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<LoginResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto)
        {
            var user = await CreateUserWithRoleAsync(registrationRequestDto.Email, 
                returnExisting: false, password: registrationRequestDto.Password);
            return await SignInAsync(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var user = await _userManager.FindByEmailAsync(loginRequestDto.UserName);
            if (user == null)
                throw new Exception("Invalid user name!");

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);
            if (!isValid)
                throw new Exception("Invalid password!");

            return await SignInAsync(user);
        }

        public async Task<IdentityUser> CreateUserWithRoleAsync(string email, bool returnExisting = true, string password = "")
        {
            // Check if user already exists
            var existingUser = await _userManager.FindByEmailAsync(email);
            if (existingUser != null)
            {
                if (returnExisting)
                    return existingUser;
                else
                    throw new Exception($"User {email} already exists!");
            }

            // Check password
            if (string.IsNullOrWhiteSpace(password))
                throw new Exception($"User {email} password is not specified!");

            // Create new user
            IdentityUser user = new()
            {
                UserName = email,
                Email = email,
                NormalizedEmail = email.ToUpper()
            };
            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                throw new Exception($"Error creating user {email}: {string.Join(", ", result.Errors.Select(err => err.Description))}");

            // Assign "User" role on initial creation
            await _userManager.AddToRoleAsync(user, Role.User);

            return user;
        }

        public async Task<LoginResponseDto> SignInAsync(IdentityUser user, string? name = null, string? picture = null)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles, name, picture);

            LoginResponseDto loginResponseDto = new()
            {
                User = new UserDto()
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = [.. roles]
                },
                Token = token
            };

            return loginResponseDto;
        }
    }
}

