using Msd.Services.EmailApi.Models.Dtos;

namespace Msd.Services.EmailApi.Services
{
    public interface IEmailService
    {
        Task<SendEmailResponseDto> SendEmailAsync(SendEmailRequestDto request);
    }
}
