// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;
using Adventure.Grains;
using Adventure.Mapping.Extensions;
using Adventure.Mapping.Mapper;
using Newtonsoft.Json;
using Orleans;
using System.Reflection;

namespace Adventure.Silo.Services;

public sealed class RoomService : BaseClusterService
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;
    private readonly IGrainFactory _grainFactory;
    public RoomService(
        IHttpContextAccessor httpContextAccessor,
        IClusterClient client,
        IGrainFactory grainFactory) :
        base(httpContextAccessor, client)
    {
        _httpContextAccessor = httpContextAccessor;
        _grainFactory = grainFactory;
    }

    public async Task<string> Create(int adventureId)
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var mapFileName = Path.Combine(path, "AdventureMap.json");

        var rand = Random.Shared;

        // Read the contents of the game file and deserialize it
        var jsonData = await File.ReadAllTextAsync(mapFileName);
        var data = JsonConvert.DeserializeObject<MapInfo>(jsonData)!;

        //Create Map
        var mapData = Mapping.Extensions.MapExtensions.CreateMap(adventureId);
        //Re-Map RoomId to remove null rooms
        var idMap = RoomMapper.MapRoomIds(mapData);
        //Create a list of rooms with the new data to add to grains
        var mappedRooms = Mapping.Extensions.MapExtensions.BuildList(mapData, adventureId, idMap);
        //Create a int array with the new RoomIds removing unknown locations
        var roomIdMap = Mapping.Extensions.MapExtensions.BuildIdMap(mapData, idMap);
        //Create a map of RoomIds to regions
        var roomRegionMap = RoomMapper.RegionMap(idMap, mapData);

        // Initialize the game world using the game data
        var rooms = new List<IRoomGrain>();
        foreach (var room in mappedRooms)
        {
            var roomGr = await MakeRoom(room);
            if (int.Parse(room.Id) >= 0)
            {
                rooms.Add(roomGr);
            }
        }

        await _client.GetGrain<IAdventureGrain>(adventureId).AddRooms(mappedRooms);
        await _client.GetGrain<IAdventureGrain>(adventureId).SetIdMap(roomIdMap);
        await _client.GetGrain<IAdventureGrain>(adventureId).SetRegionMap(roomRegionMap);

        var thingCount = 0;
        foreach (var thing in Mapping.Things.SeedData.ThingType())
        {
            var thingType = rand.Next(0, 1);
            if (thingType == 0)
            {
                var thingSeedCount = rand.Next(0, thing.MaxSeed);
                for (var i = 0; i < thingSeedCount; i++)
                {
                    var thingOnAdventure = new Thing(thingCount.ToString(), thing.Name, thing.Category, thing.Commands, thing.Damage, thing.HealthGain);
                    await MakeThing(
                        thingOnAdventure,
                        rand.Next(1, rooms.Count).ToString());
                    thingCount++;
                }
            }
        }

        var monsterCount = 0;
        foreach (var monster in Mapping.Monsters.SeedData.MonsterTypes())
        {
            var monsterType = rand.Next(0, 1);
            if (monsterType == 0)
            {
                var monsterSeedCount = rand.Next(0, monster.MaxSeed);
                for (var i = 0; i < monsterSeedCount; i++)
                {
                    var monsterOnAdventure = new MonsterInfo(monsterCount.ToString(), monster.Name, adventureId, monster.Health, monster.Damage, []);
                    await MakeMonster(
                        monsterOnAdventure,
                        rooms[rand.Next(1, rooms.Count)]);
                    monsterCount++;
                }
            }
        }

        return $"Created {mappedRooms.Count} rooms";
    }

    public async Task<string> ViewMap(int adventureId)
    {
        var adventureGrain = _grainFactory.GetGrain<IAdventureGrain>(adventureId);
        var rooms = await adventureGrain.Rooms();
        var startRoom = rooms.OrderBy(r => r.Id).FirstOrDefault();
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(startRoom.Id);

        return await roomGrain.ViewMap();
    }

    public async Task<string> Reset(int adventureId)
    {
        var players = new List<string>();
        //Maps should never have over 1000 rooms currently, so we iterate through 1000 rooms by int Id to reset any in use.
        for (var i = 0; i < 1000; i++)
        {
            var roomGrain = _grainFactory.GetGrain<IRoomGrain>($"{i}");
            if (roomGrain != null) {
                var roomPlayers = await roomGrain.ResetInfo();
                foreach (var player in roomPlayers)
                {
                    players.Add(player);
                }
            }
        }

        await Create(adventureId);

        foreach(var e in players)
        {
            var roomGrain = _grainFactory.GetGrain<IRoomGrain>("0");
            var player = _grainFactory.GetGrain<IPlayerGrain>(e);
            await player.SetRoomGrain(roomGrain);
        }


        return "Map reset and all players returned to start";
    }

    private async Task<IRoomGrain> MakeRoom(RoomInfo data)
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(data.Id);
        await roomGrain.SetInfo(data);
        return roomGrain;
    }

    private async Task MakeThing(Thing thing, string room)
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(room);
        await roomGrain.Drop(thing);
    }

    private async Task MakeMonster(MonsterInfo data, IRoomGrain room)
    {
        var monsterGrain = _grainFactory.GetGrain<IMonsterGrain>(data.Id);
        await monsterGrain.SetInfo(data);
        await monsterGrain.SetRoomGrain(room);
    }

    public async Task<bool> GetDiscovery(int roomId)
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(roomId.ToString());
        var result =  await roomGrain.GetDiscovery();
        return result;
    }
}
