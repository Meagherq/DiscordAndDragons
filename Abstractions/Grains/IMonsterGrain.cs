using Adventure.Abstractions.Info;

namespace Adventure.Abstractions.Grains;

public interface IMonsterGrain : IGrainWithStringKey
{
    // Even monsters have a name
    Task<string?> Name();
    Task SetInfo(MonsterInfo info);

    // Monsters are located in exactly one room
    Task SetRoomGrain(IRoomGrain room);
    Task<string> RoomGrain();

    Task<(string, int?)> Attack(IRoomGrain room, int damage, IPlayerGrain player);
    Task AddToAdventure(int adventureId);
}

[GenerateSerializer]
public class MonsterState
{
    [Id(0)]
    public string? roomGrain { get; set; }
    [Id(1)]
    public MonsterInfo monsterInfo { get; set; } = new();
}
