// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System.Reflection;
using Newtonsoft.Json;

namespace Adventure.Silo.StartupTasks;

public sealed class SeedAdventureGameTask : IStartupTask
{
    private readonly IGrainFactory _grainFactory;

    public SeedAdventureGameTask(IGrainFactory grainFactory) =>
        _grainFactory = grainFactory;

    async Task IStartupTask.Execute(CancellationToken cancellationToken)
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        var mapFileName = Path.Combine(path, "AdventureMap.json");

        var rand = Random.Shared;

        // Read the contents of the game file and deserialize it
        var jsonData = await File.ReadAllTextAsync(mapFileName);
        var data = JsonConvert.DeserializeObject<MapInfo>(jsonData)!;

        // Initialize the game world using the game data
        var rooms = new List<IRoomGrain>();
        foreach (var room in data.Rooms)
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
