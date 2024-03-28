// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.RegularExpressions;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace Adventure.Silo.Pages;

public sealed partial class Index
{
    private int? AdventureId;
    private string _adventureName;
    private bool createAdventureEnabled = true;
    private List<AdventureInfo> _adventures = new();
    private AdventureInfo selectedAdventure = null;
    string AdventureName
    {
        get { return _adventureName; }
        set
        {
            _adventureName = Regex.Replace(value, @"[^a-zA-Z]", "");
        }
    }
    [Inject]
    public AdventureService _adventureService { get; set; } = null!;
    [Inject]
    public AdventureLogService _adventureLogService { get; set; } = null!;

    [Inject]
    public RoomService _roomService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        _adventures = await _adventureLogService.ListAdventures();
        base.OnInitializedAsync();
    }
    private async Task CreateOrJoinAdventure()
    {
        createAdventureEnabled = true;
        if (AdventureId is null)
        {
            AdventureId = await _adventureService.Create(AdventureName);
            _adventures = await _adventureLogService.ListAdventures();
            await _roomService.Create(AdventureId.Value);
        }

        await ProtectedSessionStore.SetAsync("AdventureId", AdventureId);
        MyNavigationManager.NavigateTo($"{MyNavigationManager.BaseUri}CharacterSelection?adventureId={AdventureId}");
    }

    private void UpdateAdventureId(string? e)
    {
        AdventureId = int.TryParse(e.ToString(), out var result) ? result : null;
        if (AdventureId is not null && AdventureId.Value > 100000)
        {
            createAdventureEnabled = false;
        }
        else
        {
            createAdventureEnabled = true;
        }
    }
    private async void UpdateAdventureName(string? e)
    {
        _adventureName = e;
        if (e.Length > 0 || (AdventureId is not null && AdventureId.Value > 100000))
        {
            createAdventureEnabled = false;
        }
        else
        {
            createAdventureEnabled = true;
        }
    }

    private async Task JoinExistingAdventure(int adventureId)
    {
        await ProtectedSessionStore.SetAsync("AdventureId", adventureId);
        MyNavigationManager.NavigateTo($"{MyNavigationManager.BaseUri}CharacterSelection?adventureId={adventureId}");
    }
}
