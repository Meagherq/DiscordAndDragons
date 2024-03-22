// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Mapping.Enums;

namespace Adventure.Mapping.Models;
public class MapRoomData
{

    public MapRoomData()
    {
    }
    public int Id { get; set; }
    public int x { get; set; }
    public int y { get; set; }
    public int z { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public List<Direction> Directions { get; set; } = new List<Direction>();
    public RegionType Region { get; set; }
    public LocationType Location { get; set; }
    public int Elevation { get; set; }
    public bool EdgeOfMap { get; set; }
    public bool Ladder { get; set; }
    public bool Initialized { get; set; } = false;
    public bool Discovered { get; set; } = false;
}

public class Direction
{
    public DirectionType Name { get; set; }
    public int? id { get; set; }
}
