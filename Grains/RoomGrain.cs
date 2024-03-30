using System.Text;
using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Grains;

/// <summary>
/// Orleans grain implementation class Grain1.
/// </summary>
public class RoomGrain : Grain, IRoomGrain
{
    private readonly IPersistentState<RoomState> _state;

    protected readonly IClusterClient _client = null!;
    public RoomGrain(IClusterClient client, [PersistentState(stateName: "rooms", storageName: "rooms")]
            IPersistentState<RoomState> state)
    {
        _client = client;
        _state = state;
    }

    async Task IRoomGrain.Enter(PlayerInfo player)
    {
        _state.State.players.RemoveAll(x => x.Key == player.Key);
        _state.State.players.Add(player);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.Exit(PlayerInfo player)
    {
        _state.State.players.RemoveAll(x => x.Key == player.Key);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.Enter(MonsterInfo monster)
    {
        _state.State.monsters.RemoveAll(x => x.Id == monster.Id);
        _state.State.monsters.Add(monster);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.Exit(MonsterInfo monster)
    {
        _state.State.monsters.RemoveAll(x => x.Id == monster.Id);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.Drop(Thing thing)
    {
        _state.State.things.RemoveAll(x => x.Id == thing.Id);
        _state.State.things.Add(thing);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.Take(Thing thing)
    {
        _state.State.things.RemoveAll(x => x.Name == thing.Name);
        await _state.WriteStateAsync();
    }

    async Task IRoomGrain.SetInfo(RoomInfo info)
    {
        _state.State.description = info.Description;
        _state.State.region = info.Region;
        _state.State.location = info.Location;
        _state.State.elavation = info.Elevation;
        _state.State.map = info.Map;
        _state.State.id = info.Id;

        if (info.Region.Contains("start"))
        {
            _state.State.discovered = true;
        }

       _state.State.exits = info.Directions;

        await _state.WriteStateAsync();
    }

    async Task<List<string>> IRoomGrain.ResetInfo()
    {
        var result = new List<string>();
        _state.State.description = "";
        _state.State.region = "";
        _state.State.location = "";
        _state.State.elavation = "";
        _state.State.map = "";
        _state.State.things.Clear();
        _state.State.exits.Clear();

        foreach (var e in _state.State.players)
        {
            result.Add(e.Key);
        }

        _state.State.players.Clear();
        _state.State.monsters.Clear();
        await _state.WriteStateAsync();

        return result;
    }

    Task<Thing?> IRoomGrain.FindThing(string name) =>
        Task.FromResult(_state.State.things.FirstOrDefault(x => x.Name == name));

    Task<PlayerInfo?> IRoomGrain.FindPlayer(string name)
    {
        name = name.ToLower();
        return Task.FromResult(
            _state.State.players.FirstOrDefault(
                x => x?.Name?.ToLower()?.Contains(name) ?? false));
    }

    Task<MonsterInfo?> IRoomGrain.FindMonster(string name)
    {
        name = name.ToLower();
        return Task.FromResult(
            _state.State.monsters.FirstOrDefault(
                x => x?.Name?.ToLower()?.Contains(name) ?? false));
    }
    Task<string> IRoomGrain.Description(PlayerInfo whoisAsking)
    {
        StringBuilder builder = new();
        builder.AppendLine($"Description: {_state.State.description}");
        builder.AppendLine($"Region: {_state.State.region}");
        //builder.AppendLine($"Location: {_state.State.location}");
        //builder.AppendLine($"Elevation: {_elavation}");

        if (_state.State.exits.Count > 0)
        {
            builder.AppendLine("These exits are present:");
            foreach (var exit in _state.State.exits)
            {
                builder.Append("---").AppendLine(exit.Key);
            }
        }

        if (_state.State.things.Count > 0)
        {
            builder.AppendLine("The following things are present:");
            foreach (var thing in _state.State.things)
            {
                builder.Append("---").AppendLine(thing.Name);
            }
        }

        var others = _state.State.players.Where(pi => pi.Key != whoisAsking.Key).ToArray();
        if (others.Length > 0 || _state.State.monsters.Count > 0)
        {
            builder.AppendLine("Beware! These guys are in the room with you:");
            if (others.Length > 0)
                foreach (var player in others)
                {
                    builder.Append("---").AppendLine(player.Name);
                }
            if (_state.State.monsters.Count > 0)
                foreach (var monster in _state.State.monsters)
                {
                    builder.Append("---").AppendLine($"{monster.Name} with {monster.Health} health");
                }
        }

        return Task.FromResult(builder.ToString());
    }

    Task<string> IRoomGrain.ViewMap()
    {
        StringBuilder builder = new();
        if (!string.IsNullOrWhiteSpace(_state.State.map))
        {
            builder.AppendLine($"Map: ");
            builder.AppendLine($"{_state.State.map}");
        }

        return Task.FromResult(builder.ToString());
    }

    Task<IRoomGrain> IRoomGrain.ExitTo(string direction)
    {
        if (_state.State.exits.ContainsKey(direction))
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.exits[direction]);
            return Task.FromResult(roomGrain);
        }
        else
        {
            return null;
        }
    }

    Task<bool> IRoomGrain.GetDiscovery()
    {
        return Task.FromResult(_state.State.discovered);
    }

    Task<Dictionary<string, string>> IRoomGrain.Exits()
    {
        return Task.FromResult(_state.State.exits);
    }
}
