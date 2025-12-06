using Microsoft.AspNetCore.Mvc;
using Msd.Services.EmailApi.Models;
using Msd.Services.EmailApi.Models.Dtos;
using Msd.Services.EmailApi.Services;

namespace Msd.Services.EmailApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailController> _logger;

        public EmailController(IEmailService emailService, ILogger<EmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        /// <summary>
        /// Send an email using Azure Communication Services
        /// </summary>
        /// <param name="request">Email details including recipient, subject, and body</param>
        /// <returns>Response with message ID and status</returns>
        [HttpPost("send")]
        [ProducesResponseType(typeof(ApiResponse<SendEmailResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailRequestDto request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.To) ||
                    string.IsNullOrWhiteSpace(request.Subject) ||
                    string.IsNullOrWhiteSpace(request.Body))
                {
                    return BadRequest(new ApiResponse<object>("To, Subject, and Body are required"));
                }

                var result = await _emailService.SendEmailAsync(request);

                return Ok(new ApiResponse<SendEmailResponseDto>
                {
                    Message = "Email sent successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email");
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new ApiResponse<object>($"Failed to send email: {ex.Message}"));
            }
        }
    }
}
