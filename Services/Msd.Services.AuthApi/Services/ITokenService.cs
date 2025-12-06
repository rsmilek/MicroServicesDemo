using Microsoft.AspNetCore.Identity;

namespace Msd.Services.AuthApi.Services
{
    public interface ITokenService
    {
      string GenerateToken(IdentityUser user, IList<string> roles, string? name = null, string? picture = null);
    }
}

