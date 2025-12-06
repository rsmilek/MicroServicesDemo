namespace Msd.Services.AuthApi.Models
{
    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public UserInfo User { get; set; } = new();
    }
}

