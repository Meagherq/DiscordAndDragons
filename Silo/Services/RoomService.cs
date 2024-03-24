﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

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

    public async Task<string> Create()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var mapFileName = Path.Combine(path, "AdventureMap.json");

        var rand = Random.Shared;

        // Read the contents of the game file and deserialize it
        var jsonData = await File.ReadAllTextAsync(mapFileName);
        var data = JsonConvert.DeserializeObject<MapInfo>(jsonData)!;

        //Create Map
        var mappedRooms = Mapping.Extensions.MapExtensions.CreateMap();

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

        foreach (var thing in data.Things)
        {
            await MakeThing(thing);
        }

        foreach (var monster in data.Monsters)
        {
            await MakeMonster(
                monster,
                rooms[rand.Next(1, rooms.Count)]);
        }

        return $"Created {mappedRooms.Count} rooms";
    }

    public async Task<string> ViewMap()
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>("0");

        return await roomGrain.ViewMap();
    }

    public async Task<string> Reset()
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

        await Create();

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

    private async Task MakeThing(Thing thing)
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(thing.FoundIn);
        await roomGrain.Drop(thing);
    }

    private async Task MakeMonster(MonsterInfo data, IRoomGrain room)
    {
        var monsterGrain = _grainFactory.GetGrain<IMonsterGrain>(data.Id);
        await monsterGrain.SetInfo(data);
        await monsterGrain.SetRoomGrain(room);
    }
}
