using System.Text;
using Adventure.Abstractions;
using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;
using Adventure.Grains.Enums;
using Adventure.Grains.Models;
using Orleans.Streams;

namespace Adventure.Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private readonly IPersistentState<PlayerState> _state;

    protected readonly IClusterClient _client = null!;

    public PlayerGrain(IClusterClient client, [PersistentState(stateName: "players", storageName: "players")]
            IPersistentState<PlayerState> state)
    {
        _client = client;
        _state = state;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        return base.OnActivateAsync(cancellationToken);
    }

    Task<string?> IPlayerGrain.Name() => Task.FromResult(_state.State.myInfo?.Name);
    Task<List<long>> IPlayerGrain.DiscoveredRooms() => Task.FromResult(_state.State.discoveredRooms);

    Task<IRoomGrain> IPlayerGrain.RoomGrain()
    {
        var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
        return Task.FromResult(roomGrain);
    }


    async Task IPlayerGrain.Die()
    {
        // Drop everything
        var dropTasks = new List<Task<string?>>();
        foreach (var thing in _state.State.things.ToArray() /* New collection */)
        {
            dropTasks.Add(Drop(thing));
        }
        await Task.WhenAll(dropTasks);

        // Exit the game
        if (_state.State.roomGrain is not null && _state.State.myInfo is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            await roomGrain.Exit(_state.State.myInfo);

            _state.State.roomGrain = null;
            _state.State.killed = true;
        }
        await _state.WriteStateAsync();
    }

    private async Task<string?> Drop(Thing? thing)
    {
        if (_state.State.killed)
        {
            return await CheckAlive();
        }

        if (_state.State.roomGrain is not null && thing is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            _state.State.things.Remove(thing);
            await roomGrain.Drop(thing);
            await _state.WriteStateAsync();

            return "Okay.";
        }

        return "I don't understand.";
    }

    private async Task<string?> Take(Thing? thing)
    {
        if (_state.State.killed)
        {
            return await CheckAlive();
        }

        if (_state.State.roomGrain is not null && thing is not null)
        {
            _state.State.things.Add(thing);
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            await roomGrain.Take(thing);
            await _state.WriteStateAsync();

            return "Okay.";
        }

        return "I don't understand.";
    }

    async Task IPlayerGrain.SetInfo(string name, int adventureId)
    {
        _state.State.myInfo = new PlayerInfo(this.GetPrimaryKeyString(), name, adventureId);
        var rand = Random.Shared;

        //Everyone starts with a Pocket Knife
        _state.State.things.Add(new Thing(rand.Next(12341, 234123).ToString(), "Pocket Knife", "weapon", [""], 1, null));
        var adventure = _client.GetGrain<IAdventureGrain>(adventureId);
        if (adventure is not null)
        {
            adventure.AddPlayer(_state.State.myInfo);
        }
        await _state.WriteStateAsync();
    }

    async Task IPlayerGrain.SetRoomGrain(IRoomGrain room)
    {
        _state.State.roomGrain = room.GetPrimaryKeyString();
        room.Enter(_state.State.myInfo);
        await _state.WriteStateAsync();
    }

    private async Task<string> Go(string direction)
    {
        IRoomGrain? destination = null;
        if (_state.State.roomGrain is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            destination = await roomGrain.ExitTo(direction);
        }

        var description = new StringBuilder();

        if (_state.State.roomGrain is not null && destination is not null)
        {
            var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
            await roomGrain.Exit(_state.State.myInfo);
            _state.State.discoveredRooms.Add(int.Parse(roomGrain.GetPrimaryKeyString()));
            await destination.Enter(_state.State.myInfo);

            _state.State.roomGrain = destination.GetPrimaryKeyString();

            var desc = await destination.Description(_state.State.myInfo);
            if (desc != null)
            {
                description.Append(desc);
            }
        }
        else
        {
            description.Append("You cannot go in that direction.");
        }

        if (_state.State.things is { Count: > 0 })
        {
            description.AppendLine("You are holding the following items:");
            foreach (var thing in _state.State.things)
            {
                description.AppendLine(thing.Name);
            }
        }
        await _state.WriteStateAsync();

        return description.ToString();
    }

    private async Task<string?> CheckAlive()
    {
        if (_state.State.killed is false)
        {
            return null;
        }

        // Go to room '-2', which is the place of no return.
        var room = GrainFactory.GetGrain<IRoomGrain>("-2");
        return await room.Description(_state.State.myInfo);
    }

    private async Task<string> Attack(string target)
    {
        if (_state.State.things.Count is 0)
        {
            return "With what? Your bare hands?";
        }

        var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);

        if (_state.State.roomGrain is not null &&
            await roomGrain.FindPlayer(target) is PlayerInfo player)
        {
            if (_state.State.things.Any(t => t.Category == "weapon"))
            {
                await GrainFactory.GetGrain<IPlayerGrain>(player.Key).Die();
                await _state.WriteStateAsync();
                return $"{target} is now dead.";
            }

            return "With what? Your bare hands?";
        }

        if (_state.State.roomGrain is not null &&
            await roomGrain.FindMonster(target) is MonsterInfo monster)
        {
            var weapons = monster.KilledBy?.Join(_state.State.things, id => id, t => t.Id, (id, t) => t);
            if (_state.State.things.Any(t => t.Category == "weapon"))
            {
                string response = await GrainFactory.GetGrain<IMonsterGrain>(monster.Id).Attack(roomGrain, _state.State.things.FirstOrDefault(t => t.Category == "weapon")?.Damage.Value ?? 0, _state.State.myInfo.Name);
                await _state.WriteStateAsync();
                return response;
            }

            return "With what? Your bare hands?";
        }

        return $"I can't see {target} here. Are you sure?";
    }

    private async Task<string> Measure(string target)
    {
        if (_state.State.things.Count is 0)
        {
            return "With what? You have no measure items?";
        }

        var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);

        if (_state.State.roomGrain is not null &&
            await roomGrain.FindPlayer(target) is PlayerInfo player)
        {
            if (_state.State.things.Any(t => t.Category == "measure"))
            {
                return $"{target} is {target.Length * 6} inches tall";
            }

            return "With what? You have no measure items?";
        }

        if (_state.State.roomGrain is not null &&
            await roomGrain.FindMonster(target) is MonsterInfo monster)
        {
            if (_state.State.things.Any(t => t.Category == "measure"))
            {
                return $"{target} is {target.Length * 6} inches tall";
            }

            return "With what? You have no measure items?";
        }

        return $"I can't see {target} here. Are you sure?";
    }

    private string RemoveStopWords(string s)
    {
        var stopwords = new[] { " on ", " the ", " a " };

        StringBuilder builder = new(s);
        foreach (var word in stopwords)
        {
            builder.Replace(word, " ");
        }

        return builder.ToString();
    }

    private Thing? FindMyThing(string name) =>
        _state.State.things.FirstOrDefault(x => x.Name == name);

    private string Rest(string[] words)
    {
        StringBuilder builder = new();

        for (var i = 1; i < words.Length; ++i)
        {
            builder.Append($"{words[i]} ");
        }

        return builder.ToString().Trim().ToLower();
    }

    async Task<string?> IPlayerGrain.Play(string command)
    {
        command = RemoveStopWords(command);

        var words = command.Split(' ');
        var verb = words[0].ToLower();
        var message = "";

        if (_state.State.killed && verb is not "end")
        {
            return await CheckAlive();
        }

        if (_state.State.roomGrain is null)
        {
            return "I don't understand.";
        }

        var roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);

        if (Enum.TryParse(verb, out PlayerCommands playerCommand)) {
            message =  playerCommand switch
            {
                PlayerCommands.look =>
                    await roomGrain.Description(_state.State.myInfo),

                PlayerCommands.go => words.Length == 1
                    ? "Go where?"
                    : await Go(words[1]),

                PlayerCommands.north or PlayerCommands.south or PlayerCommands.east or PlayerCommands.west => await Go(verb),

                PlayerCommands.attack => words.Length == 1
                    ? "Attack what?"
                    : await Attack(command[(verb.Length + 1)..]),

                PlayerCommands.drop => await Drop(FindMyThing(Rest(words))),

                PlayerCommands.take => await Take(await roomGrain.FindThing(Rest(words))),

                PlayerCommands.inv or PlayerCommands.inventory =>
                    "You are carrying: " +
                    $"{string.Join(" ", _state.State.things.Select(x => x.Name))}",

                PlayerCommands.measure => words.Length == 1
                    ? "Measure what?"
                    : await Measure(command[(verb.Length + 1)..]),

                PlayerCommands.end => "",

                _ => "I don't understand"
            };
        }
        else
        {
            return "I don't understand";
        }
        roomGrain = _client.GetGrain<IRoomGrain>(_state.State.roomGrain);
        await _state.WriteStateAsync();

        var streamId = StreamId.Create(nameof(IPlayerGrain), _state.State.myInfo.AdventureId.Value);
        this.GetStreamProvider("MemoryStreams").GetStream<PlayerNotification>(_state.State.myInfo.AdventureId.Value)
            .OnNextAsync(new PlayerNotification(message, Guid.Parse(_state.State.myInfo.Key), roomGrain.GetPrimaryKeyString()))
            .Ignore();

        return message;
    }
}
