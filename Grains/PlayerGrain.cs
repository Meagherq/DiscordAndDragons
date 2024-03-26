using System.Text;
using Adventure.Abstractions.Grains;
using Adventure.Abstractions.Info;
using Adventure.Grains.Enums;
using Adventure.Grains.Extensions;

namespace Adventure.Grains;

public class PlayerGrain : Grain, IPlayerGrain
{
    private IRoomGrain? _roomGrain; // Current room
    private readonly List<Thing> _things = new(); // Things that the player is carrying

    private bool _killed = false;
    private PlayerInfo _myInfo = null!;
    protected readonly IClusterClient _client = null!;
    public PlayerGrain(IClusterClient client)
    {
        _client = client;
    }

    public override Task OnActivateAsync(CancellationToken cancellationToken)
    {
        _myInfo = new(this.GetPrimaryKeyString(), "nobody", null);
        return base.OnActivateAsync(cancellationToken);
    }

    Task<string?> IPlayerGrain.Name() => Task.FromResult(_myInfo?.Name);

    Task<IRoomGrain> IPlayerGrain.RoomGrain() => Task.FromResult(_roomGrain!);


    async Task IPlayerGrain.Die()
    {
        // Drop everything
        var dropTasks = new List<Task<string?>>();
        foreach (var thing in _things.ToArray() /* New collection */)
        {
            dropTasks.Add(Drop(thing));
        }
        await Task.WhenAll(dropTasks);

        // Exit the game
        if (_roomGrain is not null && _myInfo is not null)
        {
            await _roomGrain.Exit(_myInfo);

            _roomGrain = null;
            _killed = true;
        }
    }

    private async Task<string?> Drop(Thing? thing)
    {
        if (_killed)
        {
            return await CheckAlive();
        }

        if (_roomGrain is not null && thing is not null)
        {
            _things.Remove(thing);
            await _roomGrain.Drop(thing);

            return "Okay.";
        }

        return "I don't understand.";
    }

    private async Task<string?> Take(Thing? thing)
    {
        if (_killed)
        {
            return await CheckAlive();
        }

        if (_roomGrain is not null && thing is not null)
        {
            _things.Add(thing);
            await _roomGrain.Take(thing);

            return "Okay.";
        }

        return "I don't understand.";
    }

    Task IPlayerGrain.SetInfo(string name, int adventureId)
    {
        _myInfo = _myInfo with { Name = name };
        _myInfo = _myInfo with { AdventureId = adventureId };
        var adventure = _client.GetGrain<IAdventureGrain>(adventureId);
        if (adventure is not null)
        {
            adventure.AddPlayer(_myInfo);
        }
        return Task.CompletedTask;
    }

    Task IPlayerGrain.SetRoomGrain(IRoomGrain room)
    {
        _roomGrain = room;
        return room.Enter(_myInfo);
    }

    private async Task<string> Go(string direction)
    {
        IRoomGrain? destination = null;
        if (_roomGrain is not null)
        {
            destination = await _roomGrain.ExitTo(direction);
        }

        var description = new StringBuilder();

        if (_roomGrain is not null && destination is not null)
        {
            await _roomGrain.Exit(_myInfo);
            await destination.Enter(_myInfo);

            _roomGrain = destination;

            var desc = await destination.Description(_myInfo);
            if (desc != null)
            {
                description.Append(desc);
            }
        }
        else
        {
            description.Append("You cannot go in that direction.");
        }

        if (_things is { Count: > 0 })
        {
            description.AppendLine("You are holding the following items:");
            foreach (var thing in _things)
            {
                description.AppendLine(thing.Name);
            }
        }

        return description.ToString();
    }

    private async Task<string?> CheckAlive()
    {
        if (_killed is false)
        {
            return null;
        }

        // Go to room '-2', which is the place of no return.
        var room = GrainFactory.GetGrain<IRoomGrain>("-2");
        return await room.Description(_myInfo);
    }

    private async Task<string> Kill(string target)
    {
        if (_things.Count is 0)
        {
            return "With what? Your bare hands?";
        }

        if (_roomGrain is not null &&
            await _roomGrain.FindPlayer(target) is PlayerInfo player)
        {
            if (_things.Any(t => t.Category == "weapon"))
            {
                await GrainFactory.GetGrain<IPlayerGrain>(player.Key).Die();
                return $"{target} is now dead.";
            }

            return "With what? Your bare hands?";
        }

        if (_roomGrain is not null &&
            await _roomGrain.FindMonster(target) is MonsterInfo monster)
        {
            var weapons = monster.KilledBy?.Join(_things, id => id, t => t.Id, (id, t) => t);
            if (weapons?.Any() ?? false)
            {
                await GrainFactory.GetGrain<IMonsterGrain>(monster.Id).Kill(_roomGrain);
                return $"{target} is now dead.";
            }

            return "With what? Your bare hands?";
        }

        return $"I can't see {target} here. Are you sure?";
    }

    private async Task<string> Measure(string target)
    {
        if (_things.Count is 0)
        {
            return "With what? You have no measure items?";
        }

        if (_roomGrain is not null &&
            await _roomGrain.FindPlayer(target) is PlayerInfo player)
        {
            if (_things.Any(t => t.Category == "measure"))
            {
                return $"{target} is {target.Length * 6} inches tall";
            }

            return "With what? You have no measure items?";
        }

        if (_roomGrain is not null &&
            await _roomGrain.FindMonster(target) is MonsterInfo monster)
        {
            if (_things.Any(t => t.Category == "measure"))
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
        _things.FirstOrDefault(x => x.Name == name);

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

        if (_killed && verb is not "end")
        {
            return await CheckAlive();
        }

        if (_roomGrain is null)
        {
            return "I don't understand.";
        }

        if (Enum.TryParse(verb, out PlayerCommands playerCommand)) {
            return playerCommand switch
            {
                PlayerCommands.look =>
                    await _roomGrain.Description(_myInfo),

                PlayerCommands.go => words.Length == 1
                    ? "Go where?"
                    : await Go(words[1]),

                PlayerCommands.north or PlayerCommands.south or PlayerCommands.east or PlayerCommands.west => await Go(verb),

                PlayerCommands.kill => words.Length == 1
                    ? "Kill what?"
                    : await Kill(command[(verb.Length + 1)..]),

                PlayerCommands.drop => await Drop(FindMyThing(Rest(words))),

                PlayerCommands.take => await Take(await _roomGrain.FindThing(Rest(words))),

                PlayerCommands.inv or PlayerCommands.inventory =>
                    "You are carrying: " +
                    $"{string.Join(" ", _things.Select(x => x.Name))}",

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
    }
}
