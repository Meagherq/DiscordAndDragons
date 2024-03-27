// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Adventure.Abstractions.Info;
using Adventure.Silo.Services;

namespace Adventure.Silo.Pages;

public sealed partial class CharacterSelection
{
    private int? AdventureId;
    private List<PlayerInfo> _players = new();
    private string _playerName;
    private bool createPlayerEnabled = true;

    [Inject]
    public PlayerService _playerService { get; set; } = null!;

    [Inject]
    public AdventureService _adventureService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var url = MyNavigationManager.Uri;

        var tempAdventureId = await ProtectedSessionStore.GetAsync<string>("adventureId");

        if (int.TryParse(tempAdventureId.Value, out var _tempAdventureId))
        {
            AdventureId = _tempAdventureId;
        }

        if (AdventureId is null && url.Contains("=") && int.TryParse(url.Substring(url.IndexOf("=") + 1, 6), out var urlAdventureId))
        {
            AdventureId = urlAdventureId;
        }

        _players = await _adventureService.GetPlayers(AdventureId.Value);

        await base.OnInitializedAsync();
    }

    private async void UpdatePlayerName(string? e)
    {
        _playerName = e;
        if (e.Length > 0)
        {
            createPlayerEnabled = false;
        }
        else
        {
            createPlayerEnabled = true;
        }
    }

    private async Task CreatePlayer()
    {
        var playerId = Guid.NewGuid().ToString();
        await _playerService.CreatePlayer(_playerName, playerId, AdventureId.Value);
        await ProtectedSessionStore.SetAsync("playerId", playerId);

        MyNavigationManager.NavigateTo($"{MyNavigationManager.BaseUri}AdventureInterface?adventureId={AdventureId}");
    }
}
