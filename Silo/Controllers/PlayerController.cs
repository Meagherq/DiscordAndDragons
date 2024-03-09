// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Adventure.Silo.Models;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace Adventure.Silo.Controllers;
[Route("api/[controller]")]
[ApiController]
public class PlayerController : ControllerBase
{
    private readonly PlayerService _playerService;
    public PlayerController(PlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost("play")]
    public async Task<IActionResult> Play([FromBody] PlayerCommandDto commandDto)
    {
        var playResult = await _playerService.Command(commandDto.command, commandDto.id);

        return Ok(playResult);
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerDto createDto)
    {
        var createdResult = await _playerService.CreatePlayer(createDto.name, createDto.id);

        return CreatedAtAction(nameof(CreatePlayer), createdResult);
    }
}
