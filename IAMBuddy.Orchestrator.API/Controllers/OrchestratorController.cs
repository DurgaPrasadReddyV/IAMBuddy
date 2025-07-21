using IAMBuddy.Orchestrator.API.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class OrchestratorController : ControllerBase
{
    private readonly OrchestratorService _orchestratorService;

    public OrchestratorController(OrchestratorService orchestratorService)
    {
        _orchestratorService = orchestratorService;
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat([FromBody] string input)
    {
        var response = await _orchestratorService.GetAgentResponseAsync(input);
        return Ok(response);
    }
}