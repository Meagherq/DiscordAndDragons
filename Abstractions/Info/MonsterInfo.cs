namespace Adventure.Abstractions.Info;

[GenerateSerializer, Immutable]
public record class MonsterInfo(
    string Id = "0",
    string? Name = null,
    int? AdventureId = null,
    int Health = 2,
    int Damage = 1,
    List<string>? KilledBy = null);
