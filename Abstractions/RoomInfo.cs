// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Abstractions;
[GenerateSerializer, Immutable]
public record class RoomInfo(
    string Id,
    string Name,
    string Description,
    string Region,
    string Location,
    string Elevation,
    Dictionary<string, string> Directions,
    string? Map);
