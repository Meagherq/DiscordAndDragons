// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Adventure.Abstractions.Grains;

namespace Adventure.Grains;
public class AdventureLogGrain : Grain, IAdventureLogGrain
{
    private readonly IPersistentState<AdventureLogState> _state;

    public AdventureLogGrain([PersistentState(stateName: "adventureLog", storageName: "adventureLog")]
            IPersistentState<AdventureLogState> state)
    {
        _state = state;
    }

    public async Task AddAdventure(AdventureInfo adventure)
    {
        _state.State.adventures.Add(adventure);
        await _state.WriteStateAsync();
    }


    public async Task<List<AdventureInfo>> Adventures()
    {
        return _state.State.adventures;
    }
}
