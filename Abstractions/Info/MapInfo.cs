using Adventure.Abstractions.Info;
using Adventure.Abstractions;
[GenerateSerializer, Immutable]
public record class MapInfo(
    string Name,
    List<RoomInfo> Rooms,
    List<CategoryInfo> Categories,
    List<Thing> Things,
    List<MonsterInfo> Monsters);
