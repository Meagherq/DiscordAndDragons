// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Abstractions.Info;
using Adventure.Mapping.Descriptions;
using Adventure.Mapping.Enums;
using Adventure.Mapping.Extensions;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Mapper;
public static class RoomMapper
{
    public static Dictionary<int, int> MapRoomIds(MapRoomData[,] rooms)
    {
        var idMap = new Dictionary<int, int>();
        var roomList = new List<MapRoomData>();
        var idCount = 0;

        for (var x = 0; x < rooms.GetLength(0); x++)
        {
            for (var y = 0; y < rooms.GetLength(1); y++)
            {
                if (rooms[x, y] is not null)
                {
                    roomList.Add(rooms[x, y]);
                }
            }
        }

        foreach (var room in roomList.OrderBy(x => x.Region))
        {
            if (room is not null && room.Region != RegionType.Unknown)
            {
                try
                {
                    idMap.Add(room.Id, idCount);
                    idCount++;
                }
                catch { }
            }
        }
        return idMap;
    }
    public static List<RoomInfo> MapRooms(MapRoomData[,] rooms, int AdventureId, Dictionary<int, int> idMap)
    {
        var result = new List<RoomInfo>();
        for (var x = 0; x < rooms.GetLength(0); x++)
        {
            for (var y = 0; y < rooms.GetLength(1); y++)
            {
                if (rooms[x, y] is not null && rooms[x, y].Region != RegionType.Unknown && rooms[x, y].Directions.Any())
                {
                    var directions = new Dictionary<string, string>();
                    foreach (var e in rooms[x, y].Directions)
                    {
                        if (idMap.Any(s => s.Key == e.id))
                        {
                            var direction = "";
                            switch (e.Name)
                            {
                                case DirectionType.North:
                                    direction = "north";
                                    break;
                                case DirectionType.East:
                                    direction = "east";
                                    break;
                                case DirectionType.South:
                                    direction = "south";
                                    break;
                                case DirectionType.West:
                                    direction = "west";
                                    break;
                                default:
                                    direction = "unknown";
                                    break;
                            }
                            directions.Add(direction, idMap.First(s => s.Key == e.id).Value.ToString());
                        }
                    }
                    var map = rooms[x, y].Region == RegionType.Start ? Mapping.Extensions.MapExtensions.DrawMap(rooms) : null;
                    var room = new RoomInfo
                    (
                        idMap.First(s => s.Key == rooms[x, y].Id).Value.ToString(),
                        rooms[x, y].Name,
                        GetRandomDescription(rooms[x, y].Region),
                        GetEnumValue(rooms[x, y].Region),
                        "",
                        "",
                        directions,
                        AdventureId,
                        map
                    );

                    result.Add(room);
                }
            }
        }
        return result;
    }

    public static string GetEnumValue(RegionType region)
    {
        switch (region)
        {
            case RegionType.Start:
                return "start";
            case RegionType.Unknown:
                return "uknown";
            case RegionType.Canyon:
                return "canyon";
            case RegionType.River:
                return "river";
            case RegionType.Dune:
                return "dune";
            case RegionType.Beach:
                return "beach";
            case RegionType.Volcano:
                return "volcano";
            case RegionType.Swamp:
                return "swamp";
            case RegionType.City:
                return "city";
            case RegionType.Town:
                return "town";
            case RegionType.Village:
                return "village";
            case RegionType.Farmland:
                return "farmland";
            case RegionType.Forest:
                return "forest";
            default:
                return "default";
        }
    }
    public static string GetRandomDescription(RegionType region)
    {
        var random = new Random();
        var x = random.Next(0, 19);
        switch (region)
        {
            case RegionType.Start:
                return "You awake on a horse drawn hay trailer, you look around slowly and see another person: 'You're finally awake!'";
            case RegionType.Unknown:
                return "uknown";
            case RegionType.Canyon:
                return Canyon.Descriptions()[x];
            case RegionType.River:
                return River.Descriptions()[x];
            case RegionType.Dune:
                return Dune.Descriptions()[x];
            case RegionType.Beach:
                return Beach.Descriptions()[x];
            case RegionType.Volcano:
                return Volcano.Descriptions()[x];
            case RegionType.Swamp:
                return Swamp.Descriptions()[x];
            case RegionType.City:
                return City.Descriptions()[x];
            case RegionType.Town:
                return Town.Descriptions()[x];
            case RegionType.Village:
                return Village.Descriptions()[x];
            case RegionType.Farmland:
                return Farmland.Descriptions()[x];
            case RegionType.Forest:
                return Forest.Descriptions()[x];
            default:
                return "default";
        }
    }
}
