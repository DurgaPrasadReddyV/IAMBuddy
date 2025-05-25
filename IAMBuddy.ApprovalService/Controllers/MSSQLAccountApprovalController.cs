using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.ApprovalService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MSSQLAccountApprovalController : ControllerBase
    {
        private readonly ILogger<MSSQLAccountApprovalController> _logger;

        public MSSQLAccountApprovalController(
            ILogger<MSSQLAccountApprovalController> logger)
        {
            _logger = logger;
        }
    }
}
