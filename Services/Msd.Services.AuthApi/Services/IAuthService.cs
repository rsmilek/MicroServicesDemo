using Msd.Services.AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Msd.Services.AuthApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<IdentityUser> CreateUserWithRoleAsync(string email, bool returnExisting = true, string password = "");
        Task<LoginResponseDto> SignInAsync(IdentityUser user, string? name = null, string? picture = null);
    }
}

