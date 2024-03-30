using Adventure.Abstractions.Info;

namespace Adventure.Abstractions.Grains;

/// <summary>
/// A room is any location in a game, including outdoor locations and
/// spaces that are arguably better described as moist, cold, caverns.
/// </summary>
public interface IRoomGrain : IGrainWithStringKey
{
    // Rooms have a textual description
    Task<string> Description(PlayerInfo whoisAsking);
    Task<string> ViewMap();
    Task SetInfo(RoomInfo info);
    Task<List<string>> ResetInfo();
    Task<IRoomGrain> ExitTo(string direction);

    // Players can enter or exit a room
    Task Enter(PlayerInfo player);
    Task Exit(PlayerInfo player);

    // Monsters can enter or exit a room
    Task Enter(MonsterInfo monster);
    Task Exit(MonsterInfo monster);

    // Things can be dropped or taken from a room
    Task Drop(Thing thing);
    Task Take(Thing thing);
    Task<Thing?> FindThing(string name);

    // Players and monsters can be killed, if you have the right weapon.
    Task<PlayerInfo?> FindPlayer(string name);
    Task<MonsterInfo?> FindMonster(string name);
    Task<bool> GetDiscovery();
    Task<Dictionary<string, string>> Exits();
}

[GenerateSerializer]
public class RoomState
{
    [Id(0)]
    public string? description { get; set; }
    [Id(1)]
    public string? region { get; set; }
    [Id(2)]
    public string? location { get; set; }
    [Id(3)]
    public string? elavation { get; set; }
    [Id(4)]
    public string? map { get; set; }
    [Id(5)]
    public string? id { get; set; }
    [Id(6)]
    public bool discovered { get; set; } = false;
    [Id(7)]
    public List<PlayerInfo> players { get; set; } = new();
    [Id(8)]
    public List<MonsterInfo> monsters { get; set; } = new();
    [Id(9)]
    public List<Thing> things { get; set; } = new();
    [Id(10)]
    public Dictionary<string, string> exits { get; set; } = new();
}
