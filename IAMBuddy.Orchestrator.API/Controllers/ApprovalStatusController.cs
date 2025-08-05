using IAMBuddy.Orchestrator.API.Models;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ApprovalStatusController : ControllerBase
{
    // Simulated in-memory store (replace with DB/service in real use)
    private static List<RequestModel> _approvalStatuses = new()
    {
        new RequestModel{Name = "Approval",       Id = 1  ,Status="Pending"      }
    };

    [HttpGet("{id}")]
    public IActionResult GetApprovalStatus(int id)
    {
        var response= _approvalStatuses.Where(m => m.Id == id);
        if (response.Any())
        {
            return Ok(response.First());
        }

        return NotFound(new { Message = "Tool not found" });
    }

    // Simulate update (manager approval)
    [HttpPost("{id}/approve")]
    public IActionResult ApproveTool(int id)
    {
        var response = _approvalStatuses.Where(m => m.Id == id);
        if (response.Any())
        {
            response.First().Status = "Approved";
        }
        else
        {
            return NotFound(new { Message = "Tool not found" });
        }
        return Ok(new { Message = "Tool approved." });
    }
}