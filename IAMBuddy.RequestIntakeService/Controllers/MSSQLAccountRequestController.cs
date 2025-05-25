using IAMBuddy.RequestIntakeService.Services;
using IAMBuddy.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IAMBuddy.RequestIntakeService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MSSQLAccountRequestController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly RequestValidationService _validationService;
        private readonly TemporalClientService _temporalService;
        private readonly ILogger<MSSQLAccountRequestController> _logger;

        public MSSQLAccountRequestController(
            AppDbContext context,
            RequestValidationService validationService,
            TemporalClientService temporalService,
            ILogger<MSSQLAccountRequestController> logger)
        {
            _context = context;
            _validationService = validationService;
            _temporalService = temporalService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<MSSQLAccountRequest>> CreateAccountRequest([FromBody] MSSQLAccountRequest request)
        {
            try
            {
                _logger.LogInformation("Received account request for user: {Username}", request.Username);

                // Validate the request
                var validationResult = await _validationService.ValidateAccountRequestAsync(request);
                if (!validationResult.IsValid)
                {
                    return BadRequest(new { Errors = validationResult.Errors });
                }

                // Save to database
                _context.AccountRequests.Add(request);
                await _context.SaveChangesAsync();

                // Start Temporal workflow
                var workflowId = await _temporalService.StartAccountProvisioningWorkflowAsync(request);
                request.WorkflowId = workflowId;
                
                _context.AccountRequests.Update(request);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Account request created with ID: {RequestId}, Workflow ID: {WorkflowId}", 
                    request.Id, workflowId);

                return CreatedAtAction(nameof(GetAccountRequest), new { id = request.Id }, request);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating account request");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MSSQLAccountRequest>> GetAccountRequest(Guid id)
        {
            var request = await _context.AccountRequests.FindAsync(id);
            
            if (request == null)
            {
                return NotFound();
            }

            return request;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MSSQLAccountRequest>>> GetAccountRequests()
        {
            return await _context.AccountRequests
                .OrderByDescending(r => r.RequestedDate)
                .ToListAsync();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] WorkflowStatus status)
        {
            var request = await _context.AccountRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            request.Status = status;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccountRequest(Guid id)
        {
            var request = await _context.AccountRequests.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }

            _context.AccountRequests.Remove(request);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
