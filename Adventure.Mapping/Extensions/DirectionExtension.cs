// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Mapping.Enums;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Extensions;
public static class DirectionExtension
{
    public static List<Direction> GenerateStartDirections()
    {
        var random = new Random();
        var directionList = new List<Direction>();

        directionList.Add(new Direction { Name = DirectionType.North, id = random.Next(0, 100) > 50 ? -1 : null });
        directionList.Add(new Direction { Name = DirectionType.East, id = random.Next(0, 100) > 50 ? -1 : null });
        directionList.Add(new Direction { Name = DirectionType.South, id = random.Next(0, 100) > 50 ? -1 : null });
        directionList.Add(new Direction { Name = DirectionType.West, id = random.Next(0, 100) > 50 ? -1 : null });

        if (directionList[(int)DirectionType.North].id is null && directionList[(int)DirectionType.East].id is null && directionList[(int)DirectionType.South].id is null && directionList[(int)DirectionType.West].id is null)
        {
            return GenerateStartDirections();
        }

        return directionList;
    }
}
