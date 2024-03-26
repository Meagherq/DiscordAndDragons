// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Abstractions.Grains;

namespace Adventure.Grains;
public class AdventureLogGrain : Grain, IAdventureLogGrain
{
    private List<AdventureInfo> _adventures = new();
    public Task AddAdventure(AdventureInfo adventure)
    {
        _adventures.Add(adventure);
        return Task.CompletedTask;
    }


    public Task<List<AdventureInfo>> Adventures()
    {
        return Task.FromResult(_adventures);
    }
}
