using Adventure.Abstractions.Info;

namespace Adventure.Abstractions;

[GenerateSerializer, Immutable]
public record class Thing(
    string Id,
    string Name,
    string Category,
    List<string> Commands,
    int? Damage,
    int? HealthGain);
