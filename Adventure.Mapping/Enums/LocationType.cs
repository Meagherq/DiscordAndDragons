// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adventure.Mapping.Enums;
public enum LocationType
{
    Start, //Always needs to be 0
    Uknown,
    Entrance,
    Body,
    Exit,
}