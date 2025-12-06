namespace Msd.Services.AuthApi.Models.Dtos
{
    public class RegistrationRequestDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? Name { get; set; }
    }
}

