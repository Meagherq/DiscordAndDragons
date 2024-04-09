// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Abstractions;

namespace Adventure.Grains.Enums;
public enum PlayerCommands
{
    look,
    go,
    north,
    south,
    east,
    west,
    attack,
    drop,
    take,
    inv,
    inventory,
    measure,
    end,
    drink,
    eat,
    health
}
