using System.Text;
using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;

namespace Adventure.Grains;

/// <summary>
/// Orleans grain implementation class Grain1.
/// </summary>
public class RoomGrain : Grain, IRoomGrain
{
    // TODO: replace placeholder grain interface with actual grain
    // communication interface(s).

    private string? _description;
    private string? _region;
    private string? _location;
    private string? _elavation;
    private string? _map;
    private readonly List<PlayerInfo> _players = new();
    private readonly List<MonsterInfo> _monsters = new();
    private readonly List<Thing> _things = new();
    private readonly Dictionary<string, IRoomGrain> _exits = new();
    protected readonly IClusterClient _client = null!;
    public RoomGrain(IClusterClient client)
    {
        _client = client;
    }

    Task IRoomGrain.Enter(PlayerInfo player)
    {
        _players.RemoveAll(x => x.Key == player.Key);
        _players.Add(player);
        return Task.CompletedTask;
    }

    Task IRoomGrain.Exit(PlayerInfo player)
    {
        _players.RemoveAll(x => x.Key == player.Key);
        return Task.CompletedTask;
    }

    Task IRoomGrain.Enter(MonsterInfo monster)
    {
        _monsters.RemoveAll(x => x.Id == monster.Id);
        _monsters.Add(monster);
        return Task.CompletedTask;
    }

    Task IRoomGrain.Exit(MonsterInfo monster)
    {
        _monsters.RemoveAll(x => x.Id == monster.Id);
        return Task.CompletedTask;
    }

    Task IRoomGrain.Drop(Thing thing)
    {
        _things.RemoveAll(x => x.Id == thing.Id);
        _things.Add(thing);
        return Task.CompletedTask;
    }

    Task IRoomGrain.Take(Thing thing)
    {
        _things.RemoveAll(x => x.Name == thing.Name);
        return Task.CompletedTask;
    }

    Task IRoomGrain.SetInfo(RoomInfo info)
    {
        _description = info.Description;
        _region = info.Region;
        _location = info.Location;
        _elavation = info.Elevation;
        _map = info.Map;

        foreach (var kv in info.Directions)
        {
            _exits[kv.Key] = GrainFactory.GetGrain<IRoomGrain>(kv.Value);
        }

        return Task.CompletedTask;
    }

    Task<List<string>> IRoomGrain.ResetInfo()
    {
        var result = new List<string>();
        _description = "";
        _region = "";
        _location = "";
        _elavation = "";
        _map = "";
        _things.Clear();
        _exits.Clear();

        foreach (var e in _players)
        {
            result.Add(e.Key);
        }

        _players.Clear();
        _monsters.Clear();

        return Task.FromResult(result);
    }

    Task<Thing?> IRoomGrain.FindThing(string name) =>
        Task.FromResult(_things.FirstOrDefault(x => x.Name == name));

    Task<PlayerInfo?> IRoomGrain.FindPlayer(string name)
    {
        name = name.ToLower();
        return Task.FromResult(
            _players.FirstOrDefault(
                x => x?.Name?.ToLower()?.Contains(name) ?? false));
    }

    Task<MonsterInfo?> IRoomGrain.FindMonster(string name)
    {
        name = name.ToLower();
        return Task.FromResult(
            _monsters.FirstOrDefault(
                x => x?.Name?.ToLower()?.Contains(name) ?? false));
    }

    Task<string> IRoomGrain.Description(PlayerInfo whoisAsking)
    {
        StringBuilder builder = new();
        builder.AppendLine(_description);
        builder.AppendLine($"Region: {_region}");
        builder.AppendLine($"Location: {_location}");
        builder.AppendLine($"Elevation: {_elavation}");

        if (_exits.Count > 0)
        {
            builder.AppendLine("These exits are present:");
            foreach (var exit in _exits)
            {
                builder.Append("  ").AppendLine(exit.Key);
            }
        }

        if (_things.Count > 0)
        {
            builder.AppendLine("The following things are present:");
            foreach (var thing in _things)
            {
                builder.Append("  ").AppendLine(thing.Name);
            }
        }

        var others = _players.Where(pi => pi.Key != whoisAsking.Key).ToArray();
        if (others.Length > 0 || _monsters.Count > 0)
        {
            builder.AppendLine("Beware! These guys are in the room with you:");
            if (others.Length > 0)
                foreach (var player in others)
                {
                    builder.Append("  ").AppendLine(player.Name);
                }
            if (_monsters.Count > 0)
                foreach (var monster in _monsters)
                {
                    builder.Append("  ").AppendLine(monster.Name);
                }
        }
        if (!string.IsNullOrWhiteSpace(_map))
        {
            builder.AppendLine($"Map: ");
            builder.AppendLine($"{ _map}");
        }

        return Task.FromResult(builder.ToString());
    }

    Task<string> IRoomGrain.ViewMap()
    {
        StringBuilder builder = new();
        if (!string.IsNullOrWhiteSpace(_map))
        {
            builder.AppendLine($"Map: ");
            builder.AppendLine($"{_map}");
        }

        return Task.FromResult(builder.ToString());
    }

    Task<IRoomGrain?> IRoomGrain.ExitTo(string direction) =>
        Task.FromResult(
            _exits.ContainsKey(direction) ? _exits[direction] : null);
}
