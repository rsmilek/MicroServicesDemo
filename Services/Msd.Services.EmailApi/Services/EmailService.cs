using Azure;
using Azure.Communication.Email;
using Msd.Services.EmailApi.Models.Dtos;

namespace Msd.Services.EmailApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _senderAddress;
        private readonly EmailClient _emailClient;
        private readonly ILogger<EmailService> _logger;

        public EmailService(
            IConfiguration configuration,
            ILogger<EmailService> logger)
        {
            var connectionString = configuration["AzureCommunicationServices:ConnectionString"]
                ?? throw new ArgumentNullException("AzureCommunicationServices:ConnectionString is not configured");

            _senderAddress = configuration["AzureCommunicationServices:SenderAddress"]
                ?? throw new ArgumentNullException("AzureCommunicationServices:SenderAddress is not configured");

            _emailClient = new EmailClient(connectionString);
            _logger = logger;
        }

        public async Task<SendEmailResponseDto> SendEmailAsync(SendEmailRequestDto request)
        {
            try
            {
                var emailMessage = new EmailMessage(
                    senderAddress: _senderAddress,
                    recipientAddress: request.To,
                    content: new EmailContent(request.Subject)
                    {
                        PlainText = request.Body
                    });

                var emailSendOperation = await _emailClient.SendAsync(WaitUntil.Completed, emailMessage);

                _logger?.LogInformation("Email sent successfully. MessageId: {MessageId}, Status: {Status}",
                    emailSendOperation.Id, emailSendOperation.Value.Status);

                return new SendEmailResponseDto
                {
                    MessageId = emailSendOperation.Id,
                    Status = emailSendOperation.Value.Status.ToString()
                };
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Failed to send email to {To}", request.To);
                throw;
            }
        }
    }
}
