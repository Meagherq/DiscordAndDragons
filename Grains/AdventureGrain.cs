using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Grains;
public class AdventureGrain : Grain, IAdventureGrain
{
    private readonly IPersistentState<AdventureState> _state;

    protected readonly IClusterClient _client = null!;

    public AdventureGrain(IClusterClient client, [PersistentState(stateName: "adventure", storageName: "adventure")]
            IPersistentState<AdventureState> state)
    {
        _client = client;
        _state = state;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
    }
    public async Task AddPlayer(PlayerInfo player)
    {
        _state.State.players.RemoveAll(x => x.Key == player.Key);
        _state.State.players.Add(player);
        await _state.WriteStateAsync();
    }

    public async Task<List<RoomInfo>> AddRooms(List<RoomInfo> rooms)
    {
        _state.State.rooms.Clear();
        _state.State.rooms.AddRange(rooms);
        await _state.WriteStateAsync();
        return _state.State.rooms;
    }

    Task<string?> IAdventureGrain.Name() => Task.FromResult(_state.State.adventureInfo?.Name);
    Task<int?[,]> IAdventureGrain.IdMap() => Task.FromResult(_state.State.idMap);

    public Task<List<PlayerInfo>> Players()
    {
        return Task.FromResult(_state.State.players);
    }

    public async Task RemovePlayer(PlayerInfo player)
    {
        _state.State.players.Remove(player);
        await _state.WriteStateAsync();
    }

    public Task<List<RoomInfo>> Rooms()
    {
        return Task.FromResult(_state.State.rooms);
    }

    public async Task SetName(string name)
    {
        _state.State.adventureInfo = new AdventureInfo((int)this.GetPrimaryKeyLong(), name);
        var adventureLogGrain = _client.GetGrain<IAdventureLogGrain>(0);
        await adventureLogGrain.AddAdventure(new AdventureInfo(_state.State.adventureInfo.Key, name));
        await _state.WriteStateAsync();
    }

    public async Task SetIdMap(int?[,] idMap)
    {
        _state.State.idMap = idMap;
        await _state.WriteStateAsync();
    }

    public Task<List<MonsterInfo>> Monsters()
    {
        return Task.FromResult(_state.State.monsters);
    }

    public async Task AddMonster(MonsterInfo monster)
    {
        _state.State.monsters.RemoveAll(x => x.Id == monster.Id);
        _state.State.monsters.Add(monster);
        await _state.WriteStateAsync();
    }

    public async Task RemoveMonster(MonsterInfo monster)
    {
        _state.State.monsters.RemoveAll(x => x.Id == monster.Id);
        await _state.WriteStateAsync();
    }
}
