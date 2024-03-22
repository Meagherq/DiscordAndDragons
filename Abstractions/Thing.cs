namespace Adventure.Abstractions;

[GenerateSerializer, Immutable]
public record class Thing(
    string Id,
    string Name,
    string Category,
    string FoundIn,
    List<string> Commands);

[GenerateSerializer, Immutable]
public record class MapInfo(
    string Name,
    List<RoomInfo> Rooms,
    List<CategoryInfo> Categories,
    List<Thing> Things,
    List<MonsterInfo> Monsters);

[GenerateSerializer, Immutable]
public record class CategoryInfo(
    string Id,
    string Name,
    List<string> Commands);
