// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;
using Adventure.Mapping.Extensions;
using Adventure.Mapping.Mapper;
using Adventure.Mapping.Models;
using Newtonsoft.Json;

namespace Adventure.Silo.StartupTasks;

public sealed class SeedAdventureGameTask : IStartupTask
{
    private readonly IGrainFactory _grainFactory;

    public SeedAdventureGameTask(IGrainFactory grainFactory) =>
        _grainFactory = grainFactory;

    async Task IStartupTask.Execute(CancellationToken cancellationToken)
    {
        //var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        //var mapFileName = Path.Combine(path, "AdventureMap.json");

        //var rand = Random.Shared;

        //// Read the contents of the game file and deserialize it
        //var jsonData = await File.ReadAllTextAsync(mapFileName);
        //var data = JsonConvert.DeserializeObject<MapInfo>(jsonData)!;

        ////Generate random dimensions for the array
        //var random = new Random();
        //var x = random.Next(20, 40);
        //var y = random.Next(20, 40);
        //var z = random.Next(1, 3);
        ////BuildMapData
        //var mapData = DimensionExtensions.BuildDemensionArray(x, y, z);
        ////Check for bad data
        //var hasBadMap = DimensionExtensions.CheckForBadMap(mapData);
        ////If bad data, loop until bad data is no more - THIS IS PROBABLY WHY IT DIDN'T START!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //while (hasBadMap)
        //{
        //    //Reset map
        //    mapData = null;
        //    //Build a new map
        //    mapData = DimensionExtensions.BuildDemensionArray(x, y, z);
        //    //Check for bad map data
        //    hasBadMap = DimensionExtensions.CheckForBadMap(mapData);
        //}

        //var mappedRooms = RoomMapper.MapRooms(mapData);

        //// Initialize the game world using the game data
        //var rooms = new List<IRoomGrain>();
        //foreach (var room in mappedRooms)
        //{
        //    var roomGr = await MakeRoom(room);
        //    if (int.Parse(room.Id) >= 0)
        //    {
        //        rooms.Add(roomGr);
        //    }
        //}

        //foreach (var thing in data.Things)
        //{
        //    await MakeThing(thing);
        //}

        //foreach (var monster in data.Monsters)
        //{
        //    await MakeMonster(
        //        monster,
        //        rooms[rand.Next(1, rooms.Count)]);
        //}
    }

    private async Task<IRoomGrain> MakeRoom(RoomInfo data)
    {
        var roomGrain = _grainFactory.GetGrain<IRoomGrain>(data.Id);
        await roomGrain.SetInfo(data);
        return roomGrain;
    }

    //private async Task MakeThing(Thing thing)
    //{
    //    var roomGrain = _grainFactory.GetGrain<IRoomGrain>(thing.FoundIn);
    //    await roomGrain.Drop(thing);
    //}

    private async Task MakeMonster(MonsterInfo data, IRoomGrain room)
    {
        var monsterGrain = _grainFactory.GetGrain<IMonsterGrain>(data.Id);
        await monsterGrain.SetInfo(data);
        await monsterGrain.SetRoomGrain(room);
    }
}
