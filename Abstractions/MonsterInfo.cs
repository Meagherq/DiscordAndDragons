namespace Adventure.Abstractions;

[GenerateSerializer, Immutable]
public record class MonsterInfo(
    string Id = "0",
    string? Name = null,
    List<string>? KilledBy = null): CreatureInfo;
