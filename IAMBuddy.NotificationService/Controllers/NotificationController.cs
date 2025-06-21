using IAMBuddy.NotificationService.Services;
using IAMBuddy.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.NotificationService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly EmailService _emailService;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(EmailService emailService, ILogger<NotificationController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost("approval-request")]
        public async Task<IActionResult> SendApprovalRequest([FromBody] MSSQLApprovalRequest approvalRequest)
        {
            try
            {
                await _emailService.SendApprovalRequestAsync("","","","","");
                return Ok(new { message = "Approval request email sent successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending approval request email");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
