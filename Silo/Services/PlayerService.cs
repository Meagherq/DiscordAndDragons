﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

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
        } else
        {
            await playerGrain.Die();
            return $"{name}: Game over!";
        }
    }

    public async Task<string> CreatePlayer(string name, string id)
    {
        var playerGrain = _client.GetGrain<IPlayerGrain>(id);
        var room1 = _client.GetGrain<IRoomGrain>("0");
        await playerGrain.SetName(name);
        await playerGrain.SetRoomGrain(room1);
        var playerName = await playerGrain.Name();
        var result = await playerGrain.Play("look");

        return $"{playerName}: {result}" ?? $"{playerName}: I don't understand.";
    }
}
