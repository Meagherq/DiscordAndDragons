// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Info;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using MudBlazor;
using System.Text.RegularExpressions;

namespace Adventure.Silo.Pages;

public sealed partial class AdventureInterface
{
    private int? AdventureId;
    private Guid? PlayerId;
    private List<PlayerInfo> _players = new();
    private string _playerName;
    private bool northEnabled = true;
    private bool southEnabled = true;
    private bool eastEnabled = true;
    private bool westEnabled = true;
    private string map;
    private string playResponse;
    private string currentPlayerLocation;

    [Inject]
    public PlayerService _playerService { get; set; } = null!;

    [Inject]
    public AdventureService _adventureService { get; set; } = null!;

    [Inject]
    public RoomService _roomService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var url = MyNavigationManager.Uri;

        var tempAdventureId = await ProtectedSessionStore.GetAsync<string>("adventureId");
        var tempPlayerId = await ProtectedSessionStore.GetAsync<string>("playerId");

        if (int.TryParse(tempAdventureId.Value, out var _tempAdventureId))
        {
            AdventureId = _tempAdventureId;
        }

        if (Guid.TryParse(tempPlayerId.Value, out var _tempPlayerId))
        {
            PlayerId = _tempPlayerId;
        }

        if (AdventureId is null && url.Contains("=") && int.TryParse(url.Substring(url.IndexOf("=") + 1, 6), out var urlAdventureId))
        {
            AdventureId = urlAdventureId;
        }

        _players = await _adventureService.GetPlayers(AdventureId.Value);
        map = await _roomService.ViewMap(AdventureId.Value);
        map = map.Replace("Map: ", "");

        var response = await _playerService.Command("look", PlayerId.ToString());
        playResponse = response;
        ProcessResponse(response);

        await base.OnInitializedAsync();
    }

    private async Task MoveNorth()
    {
        var response = await _playerService.Command("north", PlayerId.ToString());
        ProcessResponse(response);
    }
    private async Task MoveSouth()
    {
        var response = await _playerService.Command("south", PlayerId.ToString());
        ProcessResponse(response);
    }
    private async Task MoveEast()
    {
        var response = await _playerService.Command("east", PlayerId.ToString());
        ProcessResponse(response);
    }
    private async Task MoveWest()
    {
        var response = await _playerService.Command("west", PlayerId.ToString());
        ProcessResponse(response);
    }

    private void ProcessResponse(string response)
    {
        playResponse = response;
        if(response.Contains("north")) northEnabled = false;
        else northEnabled = true;

        if (response.Contains("south")) southEnabled = false;
        else southEnabled = true;

        if (response.Contains("east")) eastEnabled = false;
        else eastEnabled = true;

        if (response.Contains("west")) westEnabled = false;
        else westEnabled = true;

        if (response.Contains("|"))
        {
            currentPlayerLocation = response[(response.IndexOf("|")+1)..response.LastIndexOf("|")];
        }
    }

    private string CleanLine(string input)
    {
        var test = Regex.Replace(input, "[^0-9]", "");
        return test.Replace(" ", "") ?? "";
    }

    private bool RoomDiscovered(string input)
    {
        var result =  _roomService.GetDiscovery(int.Parse(CleanLine(input)));
        return result.Result;
    }
}
