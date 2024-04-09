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
            

            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            var exits = await roomGrain.Exits();

            if (exits is not null && exits.Count > 0) {
                var directions = new List<string>();
                var rand = Random.Shared.Next(0, exits.Count);

                foreach (var e in exits)
                {
                    directions.Add(e.Key);
                }

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
    }


    async Task<(string, int?)> IMonsterGrain.Attack(IRoomGrain room, int damage, IPlayerGrain player)
    {
        if (_state.State.roomGrain is not null)
        {
            var playerGrain = _client.GetGrain<IPlayerGrain>(player.GetPrimaryKeyString());
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            if (roomGrain.GetPrimaryKeyString() != room.GetPrimaryKeyString())
            {
                return ($"{_state.State.monsterInfo.Name} snuck away. You were too slow!", null);
            }
            else
            {
                if(_state.State.monsterInfo.Health - damage <= 0)
                {
                    _state.State.monsterInfo.KilledBy.Add("Test");
                    return (await roomGrain.Exit(_state.State.monsterInfo).ContinueWith(t => $"{_state.State.monsterInfo.Name} is dead."), null);
                } 
                else 
                {
                    var t = _state.State.monsterInfo;
                    var newHealth = _state.State.monsterInfo.Health - damage;
                    var newInfo = new MonsterInfo(t.Id, t.Name, t.AdventureId, newHealth, t.Damage, t.KilledBy);
                    _state.State.monsterInfo = newInfo;
                    await _state.WriteStateAsync();

                    return ($"{_state.State.monsterInfo.Name} has taken {damage} damage from test. " +
                        $"It has {_state.State.monsterInfo.Health} health left!", _state.State.monsterInfo.Damage);
                }
            }
        }

        return ($"{_state.State.monsterInfo.Name} is already dead. " +
            "You were too slow and someone else got to him!", null);
    }
}
