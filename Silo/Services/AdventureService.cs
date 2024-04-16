// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Silo.Services;

public sealed class AdventureService : BaseClusterService
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;

    public AdventureService(
        IHttpContextAccessor httpContextAccessor, IClusterClient client) :
        base(httpContextAccessor, client)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int> Create(string name)
    {
        var random = new Random();
        var id = random.Next(100000, 999999);
        var adventureGrain = _client.GetGrain<IAdventureGrain>(id);
        await adventureGrain.SetName(name);

        return id;
    }

    public async Task<List<PlayerInfo>> GetPlayers(int adventureId)
    {
        var adventureGrain = _client.GetGrain<IAdventureGrain>(adventureId);
        return await adventureGrain.Players();
    }

    public async Task<List<MonsterInfo>> GetMonsters(int adventureId)
    {
        var adventureGrain = _client.GetGrain<IAdventureGrain>(adventureId);
        return await adventureGrain.Monsters();
    }

    public async Task<List<RoomInfo>> GetRooms(int adventureId)
    {
        var adventureGrain = _client.GetGrain<IAdventureGrain>(adventureId);
        return await adventureGrain.Rooms();
    }
    public async Task<int?[,]> GetIdMap(int adventureId)
    {
        var adventureGrain = _client.GetGrain<IAdventureGrain>(adventureId);
        return await adventureGrain.IdMap();
    }
    public async Task<Dictionary<int, int>> GetRegionMap(int adventureId)
    {
        var adventureGrain = _client.GetGrain<IAdventureGrain>(adventureId);
        return await adventureGrain.RegionMap();
    }
}
