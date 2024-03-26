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
    Task SetName(string name);
    Task<List<PlayerInfo>> Players();
    Task AddPlayer(PlayerInfo player);
    Task RemovePlayer(PlayerInfo player);
    Task<List<RoomInfo>> Rooms();
    Task<List<RoomInfo>> AddRooms(List<RoomInfo> rooms);
    Task<List<MonsterInfo>> Monsters();
    Task AddMonster(MonsterInfo monster);
    Task RemoveMonster(MonsterInfo monster);
}
