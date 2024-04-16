using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Abstractions.Info;

namespace Adventure.Abstractions.Grains;
public interface IAdventureGrain : IGrainWithIntegerKey
{
    Task<string?> Name();
    Task<int?[,]> IdMap();
    Task<Dictionary<int, int>> RegionMap();
    Task SetName(string name);
    Task SetIdMap(int?[,] idMap);
    Task SetRegionMap(Dictionary<int, int> regionMap);
    Task<List<PlayerInfo>> Players();
    Task AddPlayer(PlayerInfo player);
    Task RemovePlayer(PlayerInfo player);
    Task<List<RoomInfo>> Rooms();
    Task<List<RoomInfo>> AddRooms(List<RoomInfo> rooms);
    Task<List<MonsterInfo>> Monsters();
    Task AddMonster(MonsterInfo monster);
    Task RemoveMonster(MonsterInfo monster);
}
[GenerateSerializer]
public class AdventureState
{
    [Id(0)]
    public AdventureInfo adventureInfo { get; set; } = null!;
    [Id(1)]
    public List<PlayerInfo> players { get; set; } = new();
    [Id(2)]
    public List<MonsterInfo> monsters { get; set; } = new();
    [Id(3)]
    public List<RoomInfo> rooms { get; set; } = new();
    [Id(4)]
    public int?[,] idMap { get; set; } = null!;
    [Id(5)]
    public Dictionary<int, int> regionMap { get; set; } = null!;
}
