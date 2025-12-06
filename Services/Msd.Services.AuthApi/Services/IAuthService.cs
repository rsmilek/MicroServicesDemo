using Msd.Services.AuthApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace Msd.Services.AuthApi.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> RegisterAsync(RegistrationRequestDto registrationRequestDto);

        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequestDto);
        
        /// <summary>
        /// Creates a new user account with the specified email address and assigns a role based on the sign-in method.
        /// </summary>
        /// <param name="email">The email address to associate with the new user account. Cannot be null or empty.</param>
        /// <param name="socialSign">Indicates whether the user is signing up via a social provider.
        /// If <see langword="true"/>, the account will be created without a password.</param>
        /// <param name="password">The password for the new user account.
        /// Required if <paramref name="socialSign"/> is <see langword="false"/>; otherwise, ignored.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the created <see cref="IdentityUser"/> instance.</returns>
        Task<IdentityUser> CreateUserWithRoleAsync(string email, bool socialSign, string password = "");
        
        Task<LoginResponseDto> SignInAsync(IdentityUser user, string? name = null, string? picture = null);
    }
}

