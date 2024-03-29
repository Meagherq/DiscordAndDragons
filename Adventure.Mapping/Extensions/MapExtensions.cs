using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Abstractions.Info;
using Adventure.Mapping.Enums;
using Adventure.Mapping.Mapper;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Extensions
{
    public static class MapExtensions
    {
        private static MapRoomData[,] mapData;

        /// <summary>
        /// Creates a new Map for Adventure Game
        /// </summary>
        /// <returns></returns>
        public static MapRoomData[,] CreateMap(int adventureId)
        {
            //Generate random dimensions for the array
            var random = new Random();
            var x = random.Next(20, 40);
            var y = random.Next(20, 40);

            //BuildMapData
            var mapData = BuildDemensionArray(x, y);
            //Check for bad data
            var hasBadMap = CheckForBadMap();
            //If bad data, loop until bad data is no more - THIS IS PROBABLY WHY IT DIDN'T START!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            while (hasBadMap)
            {
                //Reset map
                mapData = null;
                //Build a new map
                mapData = BuildDemensionArray(x, y);
                //Check for bad map data
                hasBadMap = CheckForBadMap();
            }

            //Return map data sorted by Id ASC
            return mapData;
        }

        /// <summary>
        /// Convert MapRoomData array to List<RoomInfo>
        /// </summary>
        /// <param name="data"></param>
        /// <param name="adventureId"></param>
        /// <returns></returns>
        public static List<RoomInfo> BuildList(MapRoomData[,] data, int adventureId, Dictionary<int, int> idMap)
        {
            return RoomMapper.MapRooms(data, adventureId, idMap).OrderBy(x => x.Id).ToList();
        }

        /// <summary>
        /// Converts a MapRoomData array to an int? array
        /// </summary>
        /// <param name="data"></param>
        /// <param name="adventureId"></param>
        /// <returns></returns>
        public static int?[,] BuildIdMap(MapRoomData[,] data, Dictionary<int, int> idMap)
        {
            var idArray = new int?[data.GetLength(0), data.GetLength(1)];
            for (var x = 0; x < data.GetLength(0); x++)
            {
                for (var y = 0; y < data.GetLength(1); y++)
                {
                    if (data[x, y] is not null && data[x, y]?.Region != RegionType.Unknown) {
                        var id = idMap.FirstOrDefault(s => s.Key == data[x, y]?.Id).Value;
                        idArray[x, y] = data[x, y]?.Id is null ? null : id;
                    }
                }
            }
            return idArray;
        }

        /// <summary>
        /// Builds a string representation of the MapData for easy visualization
        /// </summary>
        /// <param name="temp"></param>
        /// <returns>string</returns>
        public static string DrawMap(MapRoomData[,] temp)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < temp.GetLength(0); x++)
            {
                var sbX = new StringBuilder();
                for (var y = 0; y < temp.GetLength(1); y++)
                {
                    var regionInitial = temp[x, y]?.Region.ToString()[..1].Replace("U", "");
                    var regionId = temp[x, y]?.Region != RegionType.Unknown ? temp[x, y]?.Id.ToString() : " ";
                    var textFormat = $"{regionInitial}{regionId} |";
                    sbX.Append(textFormat.PadLeft(7, char.Parse(" ")));
                }
                sb.AppendLine(sbX.ToString());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Initializes and build mapData based on given dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static MapRoomData[,] BuildDemensionArray(int x, int y)
        {
            mapData = new MapRoomData[x, y];
            IdentifyEdges();
            RegionSeed();
            GrowSeeds();
            SetStartPoint();
            DiscoverDirections();
            TrimBushes();
            FixBadRooms();
            FixRoomName();
            return mapData;
        }

        /// <summary>
        /// Identifies Edges of Map
        /// </summary>
        /// <returns></returns>
        private static void IdentifyEdges()
        {
            var roomId = 0;
            for (var x = 0; x < mapData.GetLength(0); x++)
            {
                for (var y = 0; y < mapData.GetLength(1); y++)
                {
                    var temp = new MapRoomData
                    {
                        Id = roomId,
                        Name = "",
                        Description = $"Room {x},{y}",
                        Elevation = 0,
                        Location = LocationType.Uknown,
                        Region = RegionType.Unknown,
                        x = x,
                        y = y,
                    };
                    if (x == 0 || y == 0 || x == mapData.GetLength(0) || y == mapData.GetLength(1))
                    {
                        temp.EdgeOfMap = true;
                    }
                    mapData[x, y] = temp;
                    roomId++;
                }
            }
        }

        /// <summary>
        /// Seeds all available regions into map
        /// </summary>
        /// <returns></returns>
        private static void RegionSeed()
        {
            var max_X = mapData.GetLength(0);
            var max_Y = mapData.GetLength(1);

            foreach (var region in Enum.GetValues(typeof(RegionType)))
            {
                if ((int)region > 1)
                {
                    var random = new Random();
                    var random_X = random.Next(0, max_X);
                    var random_Y = random.Next(0, max_Y);
                    mapData[random_X, random_Y].Region = (RegionType)region;
                    mapData[random_X, random_Y].Location = LocationType.Body;
                    mapData[random_X, random_Y].Initialized = true;
                }
            }
        }

        /// <summary>
        /// Identifies and sets a random Start Point in Map
        /// </summary>
        /// <returns></returns>
        private static void SetStartPoint()
        {
            var max_X = mapData.GetLength(0);
            var max_Y = mapData.GetLength(1);
            var random = new Random();
            var startSet = false;
            while (!startSet)
            {
                var start_X = random.Next(0, max_X);
                var start_Y = random.Next(0, max_Y);

                var room_North = GetDirectionRoom(mapData, DirectionType.North, start_X, start_Y); ;
                var room_South = GetDirectionRoom(mapData, DirectionType.North, start_X, start_Y);
                var room_East = GetDirectionRoom(mapData, DirectionType.North, start_X, start_Y);
                var room_West = GetDirectionRoom(mapData, DirectionType.North, start_X, start_Y);

                if (mapData[start_X, start_Y]?.Region > RegionType.Unknown &&
                    (room_East is not null ||
                    room_North is not null ||
                    room_South is not null ||
                    room_West is not null))
                {
                    mapData[start_X, start_Y].Region = RegionType.Start;
                    mapData[start_X, start_Y].Location = LocationType.Start;
                    mapData[start_X, start_Y].Initialized = true;
                    mapData[start_X, start_Y].Id = 0;
                    startSet = true;
                }
            }
        }

        /// <summary>
        /// Grow Seeded Regions to build out map rooms
        /// </summary>
        /// <returns></returns>
        private static void GrowSeeds()
        {
            var random = new Random();
            var loops = 0;
            while (loops < 150)
            {
                var seedsThisLoops = new List<RegionType>();
                for (var x = 0; x < mapData.GetLength(0); x++)
                {
                    for (var y = 0; y < mapData.GetLength(1); y++)
                    {
                        var thisMap = mapData[x, y];
                        //If the current room is not start or unknown
                        if (thisMap.Region > RegionType.Unknown && !seedsThisLoops.Contains(mapData[x, y].Region))
                        {
                            if (RegionWeight(mapData[x, y].Region))
                            {
                                var room_North = GetDirectionRoom(mapData, DirectionType.North, x, y);
                                var room_South = GetDirectionRoom(mapData, DirectionType.South, x, y);
                                var room_East = GetDirectionRoom(mapData, DirectionType.East, x, y);
                                var room_West = GetDirectionRoom(mapData, DirectionType.West, x, y);


                                if (room_North is not null && (int)room_North.Region == 1 && random.Next(0, 100) > 75)
                                {
                                    mapData[x - 1, y].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_East is not null && (int)room_East.Region == 1 && random.Next(0, 100) > 75)
                                {
                                    mapData[x, y + 1].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_South is not null && (int)room_South.Region == 1 && random.Next(0, 100) == 75)
                                {
                                    mapData[x + 1, y].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_West is not null && (int)room_West.Region == 1 && random.Next(0, 100) == 75)
                                {
                                    mapData[x, y - 1].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                            }
                        }
                    }
                }
                loops++;
            }
        }

        /// <summary>
        /// Populate available directions in each room
        /// </summary>
        /// <returns></returns>
        private static void DiscoverDirections()
        {
            for (var x = 0; x < mapData.GetLength(0); x++)
            {
                for (var y = 0; y < mapData.GetLength(1); y++)
                {
                    var room_North = GetDirectionRoom(mapData, DirectionType.North, x, y);
                    var room_South = GetDirectionRoom(mapData, DirectionType.South, x, y);
                    var room_East = GetDirectionRoom(mapData, DirectionType.East, x, y);
                    var room_West = GetDirectionRoom(mapData, DirectionType.West, x, y);

                    if (room_West is not null && mapData[x, y] is not null)
                    {
                        mapData[x, y].Directions.Add(new Direction { id = room_West.Id, Name = DirectionType.West });
                    }
                    if (room_South is not null && mapData[x, y] is not null)
                    {
                        mapData[x, y].Directions.Add(new Direction { id = room_South.Id, Name = DirectionType.South });
                    }
                    if (room_East is not null && mapData[x, y] is not null)
                    {
                        mapData[x, y].Directions.Add(new Direction { id = room_East.Id, Name = DirectionType.East });
                    }
                    if (room_North is not null && mapData[x, y] is not null)
                    {
                        mapData[x, y].Directions.Add(new Direction { id = room_North.Id, Name = DirectionType.North });
                    }
                }
            }
        }

        /// <summary>
        /// Attempt to identify and remove rooms with no available exits
        /// </summary>
        /// <returns></returns>
        private static void FixBadRooms()
        {
            for (var x = 0; x < mapData.GetLength(0); x++)
            {
                for (var y = 0; y < mapData.GetLength(1); y++)
                {
                    var room_North = GetDirectionRoom(mapData, DirectionType.North, x, y);
                    var room_South = GetDirectionRoom(mapData, DirectionType.South, x, y);
                    var room_East = GetDirectionRoom(mapData, DirectionType.East, x, y);
                    var room_West = GetDirectionRoom(mapData, DirectionType.West, x, y);

                    if (
                        (room_East is null || room_East.Region == RegionType.Unknown) &&
                        (room_North is null || room_North.Region == RegionType.Unknown) &&
                        (room_South is null || room_South.Region == RegionType.Unknown) &&
                        (room_West is null || room_West.Region == RegionType.Unknown) &&
                        mapData[x, y] is not null)
                    {
                        mapData[x, y].Region = RegionType.Unknown;
                        mapData[x, y].Initialized = false;
                    }
                }
            }
        }

        /// <summary>
        /// Remove excess rooms to make map more dynamic
        /// </summary>
        /// <returns></returns>
        private static void TrimBushes()
        {
            var loop = 0;
            while (loop < 1)
            {
                var random = new Random();
                for (var x = 0; x < mapData.GetLength(0); x++)
                {
                    for (var y = 0; y < mapData.GetLength(1); y++)
                    {
                        var room_North = GetDirectionRoom(mapData, DirectionType.North, x, y);
                        var room_South = GetDirectionRoom(mapData, DirectionType.South, x, y);
                        var room_East = GetDirectionRoom(mapData, DirectionType.East, x, y);
                        var room_West = GetDirectionRoom(mapData, DirectionType.West, x, y);

                        if (mapData[x, y] is not null && random.Next(0, 100) > 75 && mapData[x, y].Region != RegionType.Start)
                        {
                            if (room_North is null || (room_North?.Directions.Count > 1 &&
                                room_South is null) || (room_South?.Directions.Count > 1 &&
                                room_East is null) || (room_East?.Directions.Count > 1 &&
                                room_West is null) || room_West?.Directions.Count > 1)
                            {
                                if (room_North is not null) room_North.Directions.RemoveAll(s => s.id == mapData[x, y].Id);
                                if (room_East is not null) room_East.Directions.RemoveAll(s => s.id == mapData[x, y].Id);
                                if (room_South is not null) room_South.Directions.RemoveAll(s => s.id == mapData[x, y].Id);
                                if (room_West is not null) room_West.Directions.RemoveAll(s => s.id == mapData[x, y].Id);

                                mapData[x, y] = null;
                            }
                        }
                    }
                }
                loop++;
            }
        }

        /// <summary>
        /// Static method to enable weighting of seed growth
        /// </summary>
        /// <param name="region"></param>
        /// <returns>bool</returns>
        private static bool RegionWeight(RegionType region)
        {
            var random = new Random();
            switch (region)
            {
                case RegionType.Start:
                    return false;
                case RegionType.Unknown:
                    return false;
                case RegionType.Canyon:
                    return random.Next(0, 100) > 75;
                case RegionType.River:
                    return random.Next(0, 100) > 50;
                case RegionType.Dune:
                    return random.Next(0, 100) > 75;
                case RegionType.Beach:
                    return random.Next(0, 100) > 50;
                case RegionType.Volcano:
                    return random.Next(0, 100) > 75;
                case RegionType.Swamp:
                    return random.Next(0, 100) > 50;
                case RegionType.City:
                    return random.Next(0, 100) > 60;
                case RegionType.Town:
                    return random.Next(0, 100) > 75;
                case RegionType.Village:
                    return random.Next(0, 100) > 90;
                case RegionType.Farmland:
                    return random.Next(0, 100) > 40;
                case RegionType.Forest:
                    return random.Next(0, 100) > 50;
                default:
                    return random.Next(0, 100) > 50; ;
            }
        }

        /// <summary>
        /// Renames rooms based on removed data
        /// </summary>
        /// <returns></returns>
        private static void FixRoomName()
        {
            for (var x = 0; x < mapData.GetLength(0); x++)
            {
                for (var y = 0; y < mapData.GetLength(1); y++)
                {
                    if (mapData[x, y] is not null)
                    {
                        mapData[x, y].Name = mapData[x, y].Region.ToString();
                        mapData[x, y].Description = $"{mapData[x, y].Region.ToString()} |{mapData[x, y].Id}|";
                    }
                }
            }
        }

        /// <summary>
        /// Generic Method to get dimension positions for each direction relative to current position
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="direction"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private static MapRoomData? GetDirectionRoom(MapRoomData[,] arr, DirectionType direction, int x, int y, int offset = 1)
        {
            switch (direction)
            {
                case DirectionType.North:
                    return x == 0 ? null : arr[x - offset, y];
                case DirectionType.South:
                    return x + 1 < arr.GetLength(0) ? arr[x + offset, y] : null;
                case DirectionType.East:
                    return y + 1 < arr.GetLength(1) ? arr[x, y + offset] : null;
                case DirectionType.West:
                    return  y == 0 ? null : arr[x, y - offset];
                default:
                    return null;
            }
        }

        /// <summary>
        /// Check map for excess columns of rows with little or no data
        /// </summary>
        /// <returns>bool</returns>
        private static bool CheckForBadMap()
        {
            var rowsWithPotentialIssues = 0;
            for (var x = 0; x < mapData.GetLength(0); x++)
            {
                var roomsInColumn = 0;
                for (var y = 0; y < mapData.GetLength(1); y++)
                {
                    if (mapData[x, y] is null) { roomsInColumn++; }
                }
                if (roomsInColumn > (mapData.GetLength(0) / 2)) { rowsWithPotentialIssues++; }
            }

            var columnsWithPotentialIssues = 0;
            for (var y = 0; y < mapData.GetLength(1); y++)
            {
                var roomsInRow = 0;
                for (var x = 0; x < mapData.GetLength(0); x++)
                {
                    if (mapData[x, y] is null || mapData[x, y].Region == RegionType.Unknown) { roomsInRow++; }
                }
                if (roomsInRow > (mapData.GetLength(0) / 2)) { columnsWithPotentialIssues++; }
            }

            return rowsWithPotentialIssues > (mapData.GetLength(0) / 4) || columnsWithPotentialIssues > (mapData.GetLength(1) / 4);
        }
    }
}
