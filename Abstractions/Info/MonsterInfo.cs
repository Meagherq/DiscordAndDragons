namespace Adventure.Abstractions.Info;

[GenerateSerializer, Immutable]
public record class MonsterInfo(
    string Id = "0",
    string? Name = null,
    int? AdventureId = null,
    List<string>? KilledBy = null);
