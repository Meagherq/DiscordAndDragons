// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using Adventure.Abstractions.Grains;

namespace Adventure.Silo.Services;

public sealed class AdventureLogService : BaseClusterService
{
    private readonly IHttpContextAccessor _httpContextAccessor = null!;

    public AdventureLogService(
        IHttpContextAccessor httpContextAccessor, IClusterClient client) :
        base(httpContextAccessor, client)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<List<AdventureInfo>> ListAdventures()
    {
        var adventureLog = await _client.GetGrain<IAdventureLogGrain>(0).Adventures();

        return adventureLog;
    }
}
