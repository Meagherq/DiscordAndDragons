using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Adventure.Silo.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;


namespace Adventure.Silo.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AdventureController : ControllerBase
{
    private readonly AdventureService _adventureService;
    public AdventureController(AdventureService adventureService)
    {
        _adventureService = adventureService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(string adventureName)
    {
        var result = await _adventureService.Create(adventureName);

        return Ok(result);
    }

    [HttpGet("players")]
    public async Task<IActionResult> GetPlayers(int adventureId)
    {
        var result = await _adventureService.GetPlayers(adventureId);

        return Ok(result);
    }

    [HttpGet("monsters")]
    public async Task<IActionResult> GetMonsters(int adventureId)
    {
        var result = await _adventureService.GetMonsters(adventureId);

        return Ok(result);
    }

    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms(int adventureId)
    {
        var result = await _adventureService.GetRooms(adventureId);

        return Ok(result);
    }
}