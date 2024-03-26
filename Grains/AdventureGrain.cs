using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Grains;
public class AdventureGrain : Grain, IAdventureGrain
{
    private AdventureInfo _adventureInfo = null!;
    private readonly List<PlayerInfo> _players = new();
    private readonly List<MonsterInfo> _monsters = new();
    private readonly List<RoomInfo> _rooms = new();
    protected readonly IClusterClient _client = null!;

    public AdventureGrain(IClusterClient client)
    {
        _client = client;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        await base.OnActivateAsync(cancellationToken);
    }
    public Task AddPlayer(PlayerInfo player)
    {
        _players.RemoveAll(x => x.Key == player.Key);
        _players.Add(player);
        return Task.CompletedTask;
    }

    public Task<List<RoomInfo>> AddRooms(List<RoomInfo> rooms)
    {
        _rooms.Clear();
        _rooms.AddRange(rooms);
        return Task.FromResult(_rooms);
    }

    Task<string?> IAdventureGrain.Name() => Task.FromResult(_adventureInfo?.Name);

    public Task<List<PlayerInfo>> Players()
    {
        return Task.FromResult(_players);
    }

    public Task RemovePlayer(PlayerInfo player)
    {
        _players.Remove(player);
        return Task.CompletedTask;
    }

    public Task<List<RoomInfo>> Rooms()
    {
        return Task.FromResult(_rooms);
    }

    public async Task SetName(string name)
    {
        _adventureInfo = new AdventureInfo((int)this.GetPrimaryKeyLong(), name);
        var adventureLogGrain = _client.GetGrain<IAdventureLogGrain>(0);
        await adventureLogGrain.AddAdventure(new AdventureInfo(_adventureInfo.Key, name));
    }

    public Task<List<MonsterInfo>> Monsters()
    {
        return Task.FromResult(_monsters);
    }

    public Task AddMonster(MonsterInfo monster)
    {
        _monsters.RemoveAll(x => x.Id == monster.Id);
        _monsters.Add(monster);
        return Task.CompletedTask;
    }

    public Task RemoveMonster(MonsterInfo monster)
    {
        _monsters.RemoveAll(x => x.Id == monster.Id);
        return Task.CompletedTask;
    }
}
