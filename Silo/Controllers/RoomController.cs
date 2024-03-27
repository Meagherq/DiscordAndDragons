using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Adventure.Silo.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Adventure.Silo.Controllers;
[Route("api/[controller]")]
[ApiController]
public class RoomController : ControllerBase
{
    private readonly RoomService _roomService;
    public RoomController(RoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Play(int adventureId)
    {
        var playResult = await _roomService.Create(adventureId);

        return Ok(playResult);
    }

    [HttpPost("viewMap")]
    public async Task<IActionResult> ViewMap(int adventureId)
    {
        var createdResult = await _roomService.ViewMap(adventureId);

        return Ok(createdResult);
    }

    [HttpPost("resetMap")]
    public async Task<IActionResult> ResetMap(int adventureId)
    {
        var createdResult = await _roomService.Reset(adventureId);

        return Ok(createdResult);
    }
}