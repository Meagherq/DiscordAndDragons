// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Grains;
using Adventure.Grains.Models;
using Orleans.Streams;

namespace Adventure.Silo.Services;

public sealed class PlayerService : BaseClusterService
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;

    public PlayerService(
        IHttpContextAccessor httpContextAccessor, IClusterClient client) :
        base(httpContextAccessor, client)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> Command(string command, string id)
    {
        var playerGrain = _client.GetGrain<IPlayerGrain>(id);

        var result = await playerGrain.Play(command);
        var name = await playerGrain.Name();
        if (result is not "")
        {
            return $"{name}: {result}" ?? $"{name}: I don't understand.";
        }
        else
        {
            await playerGrain.Die();
            return $"{name}: Game over!";
        }
    }

    public async Task<string> CreatePlayer(string name, string id, int adventureId)
    {
        var playerGrain = _client.GetGrain<IPlayerGrain>(id);
        var room1 = _client.GetGrain<IRoomGrain>("0");
        await playerGrain.SetInfo(name, adventureId);
        await playerGrain.SetRoomGrain(room1);
        var playerName = await playerGrain.Name();
        var result = await playerGrain.Play("look");

        return $"{playerName}: {result}" ?? $"{playerName}: I don't understand.";
    }

    public async Task<List<long>> DiscoveredRooms(string id)
    {
        var playerGrain = _client.GetGrain<IPlayerGrain>(id);

        return await playerGrain.DiscoveredRooms();
    }

    public Task<StreamSubscriptionHandle<PlayerNotification>> SubscribeAsync(
        int ownerKey, Func<PlayerNotification, Task> action) =>
        _client.GetStreamProvider("MemoryStreams")
            .GetStream<PlayerNotification>(ownerKey)
            .SubscribeAsync(new VoteObserver(action));
}

sealed file class VoteObserver : IAsyncObserver<PlayerNotification>
    {
        private readonly Func<PlayerNotification, Task> _onNext;

        public VoteObserver(
            Func<PlayerNotification, Task> action)
        {
            _onNext = action;
        }

        public Task OnCompletedAsync() => Task.CompletedTask;

        public Task OnErrorAsync(Exception ex)
        {
            return Task.CompletedTask;
        }

        public Task OnNextAsync(
            PlayerNotification item,
            StreamSequenceToken? token = null) =>
            _onNext(item);
    }
