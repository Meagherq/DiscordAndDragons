namespace Adventure.Abstractions.Info;

[GenerateSerializer, Immutable]
public record class PlayerInfo(
    string Key,
    string? Name,
    int? AdventureId);
