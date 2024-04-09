using Adventure.Abstractions.Info;

namespace Adventure.Abstractions.Grains;

/// <summary>
/// A player is, well, there's really no other good name...
/// </summary>
public interface IPlayerGrain : IGrainWithStringKey
{
    // Players have names
    Task<string?> Name();
    Task SetInfo(string name, int adventureId);

    // Each player is located in exactly one room
    Task SetRoomGrain(IRoomGrain room);
    Task<IRoomGrain> RoomGrain();

    // Until Death comes knocking
    Task Die();

    // A Player takes his turn by calling Play with a command
    Task<string?> Play(string command);
    Task<List<long>> DiscoveredRooms();
    Task Attack(int damage);
}

[GenerateSerializer]
public class PlayerState
{
    [Id(0)]
    public string? roomGrain { get; set; }
    [Id(1)]
    public List<Thing> things { get; set; } = new();
    [Id(2)]
    public bool killed { get; set; } = false;
    [Id(3)]
    public PlayerInfo myInfo { get; set; } = null!;
    [Id(4)]
    public List<long> discoveredRooms { get; set; } = new();
    [Id(5)]
    public int health { get; set; } = 20;
}
