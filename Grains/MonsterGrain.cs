using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;
using Microsoft.VisualBasic;

namespace Adventure.Grains;

public class MonsterGrain : Grain, IMonsterGrain
{
    private readonly IPersistentState<MonsterState> _state;

    protected readonly IClusterClient _client = null!;
    public MonsterGrain(IClusterClient client, [PersistentState(stateName: "monster", storageName: "monster")]
            IPersistentState<MonsterState> state)
    {
        _client = client;
        _state = state;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _state.State.monsterInfo = _state.State.monsterInfo with { Id = this.GetPrimaryKeyString() };

        RegisterTimer(
            _ => Move(),
            null!,
            TimeSpan.FromSeconds(150),
            TimeSpan.FromMinutes(150));

        return base.OnActivateAsync(cancellationToken);
    }

    async Task IMonsterGrain.SetInfo(MonsterInfo info)
    {
        _state.State.monsterInfo = info;
        var adventure = _client.GetGrain<IAdventureGrain>(info.AdventureId.Value);
        if (adventure is not null)
        {
            adventure.AddMonster(_state.State.monsterInfo);
        }
        await _state.WriteStateAsync();
    }

    async Task IMonsterGrain.AddToAdventure(int adventureId)
    {
        _client.GetGrain<IAdventureGrain>(adventureId).AddMonster(_state.State.monsterInfo);
        await _state.WriteStateAsync();
    }

    Task<string?> IMonsterGrain.Name() => Task.FromResult(_state.State.monsterInfo.Name);

    async Task IMonsterGrain.SetRoomGrain(IRoomGrain room)
    {
        if (_state.State.roomGrain is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            await roomGrain.Exit(_state.State.monsterInfo);
        }

        _state.State.roomGrain = room.GetPrimaryKeyString();
        var tempRoom = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
        await tempRoom.Enter(_state.State.monsterInfo);
    }

    Task<string> IMonsterGrain.RoomGrain() => Task.FromResult(_state.State.roomGrain!);

    private async Task Move()
    {
        if (_state.State.roomGrain is not null)
        {
            var directions = new[] { "north", "south", "west", "east" };
            var rand = Random.Shared.Next(0, 4);

            var room = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);

            var nextRoom = await room.ExitTo(directions[rand]);
            if (nextRoom is null)
            {
                return;
            }

            await room.Exit(_state.State.monsterInfo);
            await nextRoom.Enter(_state.State.monsterInfo);

            _state.State.roomGrain = nextRoom.GetPrimaryKeyString();
            await _state.WriteStateAsync();
        }
    }


    Task<string> IMonsterGrain.Kill(IRoomGrain room)
    {
        if (_state.State.roomGrain is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            return roomGrain != room
                ? Task.FromResult($"{_state.State.monsterInfo.Name} snuck away. You were too slow!")
                : roomGrain.Exit(_state.State.monsterInfo)
                    .ContinueWith(t => $"{_state.State.monsterInfo.Name} is dead.");
        }

        return Task.FromResult(
            $"{_state.State.monsterInfo.Name} is already dead. " +
            "You were too slow and someone else got to him!");
    }
}
