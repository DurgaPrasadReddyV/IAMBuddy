using Microsoft.AspNetCore.Mvc;

namespace IAMBuddy.ProvisioningService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MSSQLAccountProvisioningController : ControllerBase
    {
        private readonly ILogger<MSSQLAccountProvisioningController> _logger;

        public MSSQLAccountProvisioningController(
            ILogger<MSSQLAccountProvisioningController> logger)
        {
            _logger = logger;
        }
    }
}
