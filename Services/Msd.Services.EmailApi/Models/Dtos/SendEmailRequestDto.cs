namespace Msd.Services.EmailApi.Models.Dtos
{
    public class SendEmailRequestDto
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
    }
}
