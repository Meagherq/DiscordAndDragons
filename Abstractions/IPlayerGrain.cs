namespace Adventure.Abstractions;

/// <summary>
/// A player is, well, there's really no other good name...
/// </summary>
public interface IPlayerGrain : IGrainWithStringKey
{
    // Players have names
    Task<string?> Name();
    Task SetName(string name);

    // Each player is located in exactly one room
    Task SetRoomGrain(IRoomGrain room);
    Task<IRoomGrain> RoomGrain();

    // Until Death comes knocking
    Task Die();

    // A Player takes his turn by calling Play with a command
    Task<string?> Play(string command);
}