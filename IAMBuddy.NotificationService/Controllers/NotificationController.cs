using IAMBuddy.NotificationService.Services;
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
    }
}
