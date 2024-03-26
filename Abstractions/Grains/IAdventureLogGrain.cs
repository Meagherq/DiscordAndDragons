namespace Adventure.Abstractions.Grains;
public interface IAdventureLogGrain : IGrainWithIntegerKey
{
    Task<List<AdventureInfo>> Adventures();
    Task AddAdventure(AdventureInfo adventure);
}
