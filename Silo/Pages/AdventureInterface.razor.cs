// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Info;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using MudBlazor;
using System.Text.RegularExpressions;
using Orleans.Streams;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using Adventure.Grains.Models;
using Adventure.Grains.Enums;

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
    private StreamSubscriptionHandle<PlayerNotification>? subscription;
    private int?[,] _adventureIdMap;
    private Dictionary<int, int> _regionMap;
    private List<long> _discoveredRooms = new();
    private string? _playerCommandText;

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
        _adventureIdMap = await _adventureService.GetIdMap(AdventureId.Value);
        _regionMap = await _adventureService.GetRegionMap(AdventureId.Value);
        map = await _roomService.ViewMap(AdventureId.Value);
        map = map.Replace("Map: ", "");

        await LoadPlayerData();

        subscription = await _playerService.SubscribeAsync(
            AdventureId.Value,
            notification => InvokeAsync(
                () => HandleNotificationAsync(notification)));

        await base.OnInitializedAsync();
    }

    private async Task LoadPlayerData()
    {
        var response = await _playerService.Command("look", PlayerId.ToString());
        playResponse = response;
        ProcessResponse(response);
        _discoveredRooms = await _playerService.DiscoveredRooms(PlayerId.ToString());
    }

    private async Task PlayerCommand(PlayerCommands command)
    {
        if (!string.IsNullOrWhiteSpace(_playerCommandText))
        {
            await _playerService.Command($"{command.ToString()} {_playerCommandText}", PlayerId.ToString());
        }
        else
        {
            await _playerService.Command(command.ToString(), PlayerId.ToString());
        }
    }

    private void ProcessResponse(string response)
    {
        if(response.Contains("north")) northEnabled = false;
        else northEnabled = true;

        if (response.Contains("south")) southEnabled = false;
        else southEnabled = true;

        if (response.Contains("east")) eastEnabled = false;
        else eastEnabled = true;

        if (response.Contains("west")) westEnabled = false;
        else westEnabled = true;
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

    private Task HandleNotificationAsync(PlayerNotification notification)
    {
        playResponse = notification.message;
        if (notification.playerId == PlayerId) {
            ProcessResponse(notification.message);
            currentPlayerLocation = notification.roomId;
            var roomId = int.Parse(notification.roomId);
            if (!_discoveredRooms.Contains(roomId))
            {
                _discoveredRooms.Add(roomId);
            }
        }
        else
        {
            eastEnabled = true;
            westEnabled = true;
            northEnabled = true;
            southEnabled = true;
        }
        StateHasChanged();
        return Task.CompletedTask;
    }

    private async void UpdatePlayerCommandText(string? e)
    {
        _playerCommandText = e;
    }
}
