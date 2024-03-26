using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Adventure.Silo.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Adventure.Silo.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdventureLog : ControllerBase
{
    private readonly AdventureLogService _adventureLogService;
    public AdventureLog(AdventureLogService adventureLogService)
    {
        _adventureLogService = adventureLogService;
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var playResult = await _adventureLogService.ListAdventures();

        return Ok(playResult);
    }
}
