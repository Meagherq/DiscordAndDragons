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
enum PlayerCommands
{
    look,
    go,
    north,
    south,
    east,
    west,
    kill,
    drop,
    take,
    inv,
    inventory,
    measure,
    end
}
