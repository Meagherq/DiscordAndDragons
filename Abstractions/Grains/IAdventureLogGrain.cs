using Adventure.Abstractions.Info;

namespace Adventure.Abstractions.Grains;
public interface IAdventureLogGrain : IGrainWithIntegerKey
{
    Task<List<AdventureInfo>> Adventures();
    Task AddAdventure(AdventureInfo adventure);
}

[GenerateSerializer]
public class AdventureLogState
{
    [Id(0)]
    public List<AdventureInfo> adventures { get; set; } = new();
}
