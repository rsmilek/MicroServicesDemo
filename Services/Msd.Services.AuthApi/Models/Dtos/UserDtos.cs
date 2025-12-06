namespace Msd.Services.AuthApi.Models.Dtos
{
    public class UserDto
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Picture { get; set; }
        public List<string> Roles { get; set; } = [];
    }
}

