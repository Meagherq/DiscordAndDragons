[GenerateSerializer, Immutable]
public record class CategoryInfo(
    string Id,
    string Name,
    List<string> Commands);
