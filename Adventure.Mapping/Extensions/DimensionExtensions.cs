using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adventure.Mapping.Enums;
using Adventure.Mapping.Models;

namespace Adventure.Mapping.Extensions
{
    public static class DimensionExtensions
    {
        public static MapRoomData[,] BuildDemensionArray(int x, int y, int z)
        {
            var dimensionArray = new MapRoomData[x, y];

            var edges = IdentifyEdges(dimensionArray);
            var seeds = RegionSeed(edges);
            var GrownData = GrowSeeds(seeds);
            var final = SetStartPoint(GrownData);
            var final1 = DiscoverDirections(final);
            var finalfinal = TrimBushes(final1);
            var finalfinalfinal = FixBadRooms(finalfinal);
            var finalfinalfinalfinal = FixRoomName(finalfinalfinal);
            return finalfinalfinalfinal;
        }

        public static MapRoomData[,] IdentifyEdges(MapRoomData[,] arr)
        {
            var roomId = 0;
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
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
                    if (x == 0 || y == 0 || x == arr.GetLength(0) || y == arr.GetLength(1))
                    {
                        temp.EdgeOfMap = true;
                    }
                    arr[x, y] = temp;
                    roomId++;
                }
            }
            return arr;
        }

        public static MapRoomData[,] RegionSeed(MapRoomData[,] arr)
        {
            var max_X = arr.GetLength(0);
            var max_Y = arr.GetLength(1);

            foreach (var region in Enum.GetValues(typeof(RegionType)))
            {
                if ((int)region > 1)
                {
                    var random = new Random();
                    var random_X = random.Next(0, max_X);
                    var random_Y = random.Next(0, max_Y);
                    arr[random_X, random_Y].Region = (RegionType)region;
                    arr[random_X, random_Y].Location = LocationType.Body;
                    arr[random_X, random_Y].Initialized = true;
                }
            }

            return arr;
        }

        private static MapRoomData[,] SetStartPoint(MapRoomData[,] arr)
        {
            var max_X = arr.GetLength(0);
            var max_Y = arr.GetLength(1);
            var random = new Random();
            var startSet = false;
            while (!startSet)
            {
                var start_X = random.Next(0, max_X);
                var start_Y = random.Next(0, max_Y);

                var room_North = GetDirectionRoom(arr, DirectionType.North, start_X, start_Y); ;
                var room_South = GetDirectionRoom(arr, DirectionType.North, start_X, start_Y);
                var room_East = GetDirectionRoom(arr, DirectionType.North, start_X, start_Y);
                var room_West = GetDirectionRoom(arr, DirectionType.North, start_X, start_Y);

                if (arr[start_X, start_Y]?.Region > RegionType.Unknown &&
                    (room_East is not null ||
                    room_North is not null ||
                    room_South is not null ||
                    room_West is not null))
                {
                    arr[start_X, start_Y].Region = RegionType.Start;
                    arr[start_X, start_Y].Location = LocationType.Start;
                    arr[start_X, start_Y].Initialized = true;
                    arr[start_X, start_Y].Id = 0;
                    startSet = true;
                }
            }

            return arr;
        }

        private static MapRoomData[,] GrowSeeds(MapRoomData[,] arr)
        {
            var random = new Random();
            var loops = 0;
            while (loops < 150)
            {
                var seedsThisLoops = new List<RegionType>();
                for (var x = 0; x < arr.GetLength(0); x++)
                {
                    for (var y = 0; y < arr.GetLength(1); y++)
                    {
                        var thisMap = arr[x, y];
                        //If the current room is not start or unknown
                        if (thisMap.Region > RegionType.Unknown && !seedsThisLoops.Contains(arr[x, y].Region))
                        {
                            if (RegionWeight(arr[x, y].Region))
                            {
                                var room_North = GetDirectionRoom(arr, DirectionType.North, x, y);
                                var room_South = GetDirectionRoom(arr, DirectionType.South, x, y);
                                var room_East = GetDirectionRoom(arr, DirectionType.East, x, y);
                                var room_West = GetDirectionRoom(arr, DirectionType.West, x, y);


                                if (room_North is not null && (int)room_North.Region == 1 && random.Next(0, 100) > 75)
                                {
                                    arr[x - 1, y].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_East is not null && (int)room_East.Region == 1 && random.Next(0, 100) > 75)
                                {
                                    arr[x, y + 1].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_South is not null && (int)room_South.Region == 1 && random.Next(0, 100) == 75)
                                {
                                    arr[x + 1, y].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                                if (room_West is not null && (int)room_West.Region == 1 && random.Next(0, 100) == 75)
                                {
                                    arr[x, y - 1].Region = thisMap.Region;
                                    seedsThisLoops.Add(thisMap.Region);
                                    break;
                                }
                            }
                        }
                    }
                }
                loops++;
            }

            return arr;
        }

        private static MapRoomData[,] DiscoverDirections(MapRoomData[,] arr)
        {
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    var room_North = GetDirectionRoom(arr, DirectionType.North, x, y);
                    var room_South = GetDirectionRoom(arr, DirectionType.South, x, y);
                    var room_East = GetDirectionRoom(arr, DirectionType.East, x, y);
                    var room_West = GetDirectionRoom(arr, DirectionType.West, x, y);

                    if (room_West is not null && arr[x, y] is not null)
                    {
                        arr[x, y].Directions.Add(new Direction { id = room_West.Id, Name = DirectionType.West });
                    }
                    if (room_South is not null && arr[x, y] is not null)
                    {
                        arr[x, y].Directions.Add(new Direction { id = room_South.Id, Name = DirectionType.South });
                    }
                    if (room_East is not null && arr[x, y] is not null)
                    {
                        arr[x, y].Directions.Add(new Direction { id = room_East.Id, Name = DirectionType.East });
                    }
                    if (room_North is not null && arr[x, y] is not null)
                    {
                        arr[x, y].Directions.Add(new Direction { id = room_North.Id, Name = DirectionType.North });
                    }
                }
            }

            return arr;
        }

        private static MapRoomData[,] FixBadRooms(MapRoomData[,] arr)
        {
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    var room_North = GetDirectionRoom(arr, DirectionType.North, x, y);
                    var room_South = GetDirectionRoom(arr, DirectionType.South, x, y);
                    var room_East = GetDirectionRoom(arr, DirectionType.East, x, y);
                    var room_West = GetDirectionRoom(arr, DirectionType.West, x, y);

                    if (
                        (room_East is null || room_East.Region == RegionType.Unknown) &&
                        (room_North is null || room_North.Region == RegionType.Unknown) &&
                        (room_South is null || room_South.Region == RegionType.Unknown) &&
                        (room_West is null || room_West.Region == RegionType.Unknown) &&
                        arr[x, y] is not null)
                    {
                        arr[x, y].Region = RegionType.Unknown;
                        arr[x, y].Initialized = false;
                    }
                }
            }

            return arr;
        }

        private static MapRoomData[,] TrimBushes(MapRoomData[,] arr)
        {
            var loop = 0;
            while (loop < 1)
            {
                var random = new Random();
                for (var x = 0; x < arr.GetLength(0); x++)
                {
                    for (var y = 0; y < arr.GetLength(1); y++)
                    {
                        var room_North = GetDirectionRoom(arr, DirectionType.North, x, y);
                        var room_South = GetDirectionRoom(arr, DirectionType.South, x, y);
                        var room_East = GetDirectionRoom(arr, DirectionType.East, x, y);
                        var room_West = GetDirectionRoom(arr, DirectionType.West, x, y);

                        if (arr[x, y] is not null && random.Next(0, 100) > 75 && arr[x, y].Region != RegionType.Start)
                        {
                            if (room_North is null || (room_North?.Directions.Count > 1 &&
                                room_South is null) || (room_South?.Directions.Count > 1 &&
                                room_East is null) || (room_East?.Directions.Count > 1 &&
                                room_West is null) || room_West?.Directions.Count > 1)
                            {
                                if (room_North is not null) room_North.Directions.RemoveAll(s => s.id == arr[x, y].Id);
                                if (room_East is not null) room_East.Directions.RemoveAll(s => s.id == arr[x, y].Id);
                                if (room_South is not null) room_South.Directions.RemoveAll(s => s.id == arr[x, y].Id);
                                if (room_West is not null) room_West.Directions.RemoveAll(s => s.id == arr[x, y].Id);

                                arr[x, y] = null;
                            }
                        }
                    }
                }
                loop++;
            }

            return arr;
        }

        public static int CheckForBadData(MapRoomData[,] arr)
        {
            var badData = 0;
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    var room_North = GetDirectionRoom(arr, DirectionType.North, x, y);
                    var room_South = GetDirectionRoom(arr, DirectionType.South, x, y);
                    var room_East = GetDirectionRoom(arr, DirectionType.East, x, y);
                    var room_West = GetDirectionRoom(arr, DirectionType.West, x, y);

                    if (room_East is null && room_North is null && room_South is null && room_West is null && arr[x, y] is not null)
                    {
                        badData++;
                    }
                }
            }

            return badData;
        }

        public static string DrawMap(MapRoomData[,] arr)
        {
            var sb = new StringBuilder();
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                var sbX = new StringBuilder();
                for (var y = 0; y < arr.GetLength(1); y++)
                {

                    sbX.Append($"{arr[x, y]?.Region.ToString().Substring(0, 1).Replace("U", " ")}{(arr[x, y]?.Region != RegionType.Unknown ? arr[x, y]?.Id : "")} |".PadLeft(7));
                }
                sb.AppendLine(sbX.ToString());
            }
            return sb.ToString();
        }

        public static bool RegionWeight(RegionType region)
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

        private static MapRoomData[,] FixRoomName(MapRoomData[,] arr)
        {
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] is not null)
                    {
                        arr[x, y].Name = arr[x, y].Region.ToString();
                        arr[x, y].Description = $"{arr[x, y].Region.ToString()} {arr[x, y].Id}";
                    }
                }
            }

            return arr;
        }

        private static MapRoomData? GetDirectionRoom(MapRoomData[,] arr, DirectionType direction, int x, int y, int? offset = 1)
        {
            switch (direction)
            {
                case DirectionType.North:
                    return x == 0 ? null : arr[x - offset.Value, y];
                case DirectionType.South:
                    return x + 1 < arr.GetLength(0) ? arr[x + offset.Value, y] : null;
                case DirectionType.East:
                    return y + 1 < arr.GetLength(1) ? arr[x, y + offset.Value] : null;
                case DirectionType.West:
                    return  y == 0 ? null : arr[x, y - offset.Value];
                default:
                    return null;
            }
        }

        public static bool CheckForBadMap(MapRoomData[,] arr)
        {
            var rowsWithPotentialIssues = 0;
            for (var x = 0; x < arr.GetLength(0); x++)
            {
                var roomsInColumn = 0;
                for (var y = 0; y < arr.GetLength(1); y++)
                {
                    if (arr[x, y] is null) { roomsInColumn++; }
                }
                if (roomsInColumn > (arr.GetLength(0) / 2)) { rowsWithPotentialIssues++; }
            }

            var columnsWithPotentialIssues = 0;
            for (var y = 0; y < arr.GetLength(1); y++)
            {
                var roomsInRow = 0;
                for (var x = 0; x < arr.GetLength(0); x++)
                {
                    if (arr[x, y] is null || arr[x, y].Region == RegionType.Unknown) { roomsInRow++; }
                }
                if (roomsInRow > (arr.GetLength(0) / 2)) { columnsWithPotentialIssues++; }
            }

            return rowsWithPotentialIssues > 5 || columnsWithPotentialIssues > 5;
        }
    }
}
