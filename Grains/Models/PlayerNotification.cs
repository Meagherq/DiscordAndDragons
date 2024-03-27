// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Grains.Models;
[Immutable]
[GenerateSerializer]
public record class PlayerNotification(string message, Guid? playerId);
