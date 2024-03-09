namespace Adventure.Abstractions;

[GenerateSerializer, Immutable]
public record class PlayerInfo(
    string Key,
    string? Name);
