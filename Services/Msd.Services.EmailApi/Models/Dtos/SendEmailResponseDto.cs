namespace Msd.Services.EmailApi.Models.Dtos
{
    public class SendEmailResponseDto
    {
        public required string MessageId { get; set; }
        public required string Status { get; set; }
    }
}
