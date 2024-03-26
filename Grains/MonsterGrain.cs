using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Grains;

public class MonsterGrain : Grain, IMonsterGrain
{
    private MonsterInfo _monsterInfo = new();
    private IRoomGrain? _roomGrain; // Current room
    protected readonly IClusterClient _client = null!;
    public MonsterGrain(IClusterClient client)
    {
        _client = client;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _monsterInfo = _monsterInfo with { Id = this.GetPrimaryKeyString() };

        RegisterTimer(
            _ => Move(),
            null!,
            TimeSpan.FromSeconds(150),
            TimeSpan.FromMinutes(150));

        return base.OnActivateAsync(cancellationToken);
    }

    Task IMonsterGrain.SetInfo(MonsterInfo info)
    {
        _monsterInfo = info;
        var adventure = _client.GetGrain<IAdventureGrain>(info.AdventureId.Value);
        if (adventure is not null)
        {
            adventure.AddMonster(_monsterInfo);
        }
        return Task.CompletedTask;
    }

    Task IMonsterGrain.AddToAdventure(int adventureId)
    {
        _client.GetGrain<IAdventureGrain>(adventureId).AddMonster(_monsterInfo);
        return Task.CompletedTask;
    }

    Task<string?> IMonsterGrain.Name() => Task.FromResult(_monsterInfo.Name);

    async Task IMonsterGrain.SetRoomGrain(IRoomGrain room)
    {
        if (_roomGrain is not null)
        {
            await _roomGrain.Exit(_monsterInfo);
        }

        _roomGrain = room;
        await _roomGrain.Enter(_monsterInfo);
    }

    Task<IRoomGrain> IMonsterGrain.RoomGrain() => Task.FromResult(_roomGrain!);

    private async Task Move()
    {
        if (_roomGrain is not null)
        {
            var directions = new[] { "north", "south", "west", "east" };
            var rand = Random.Shared.Next(0, 4);

            var nextRoom = await _roomGrain.ExitTo(directions[rand]);
            if (nextRoom is null)
            {
                return;
            }

            await _roomGrain.Exit(_monsterInfo);
            await nextRoom.Enter(_monsterInfo);

            _roomGrain = nextRoom;
        }
    }


    Task<string> IMonsterGrain.Kill(IRoomGrain room)
    {
        if (_roomGrain is not null)
        {
            return _roomGrain.GetPrimaryKey() != room.GetPrimaryKey()
                ? Task.FromResult($"{_monsterInfo.Name} snuck away. You were too slow!")
                : _roomGrain.Exit(_monsterInfo)
                    .ContinueWith(t => $"{_monsterInfo.Name} is dead.");
        }

        return Task.FromResult(
            $"{_monsterInfo.Name} is already dead. " +
            "You were too slow and someone else got to him!");
    }
}
