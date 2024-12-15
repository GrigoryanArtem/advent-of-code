﻿using Puzzles.Base.Entites;

namespace Puzzles.Runner._2024;

[Puzzle("Warehouse Woes", 15, 2024)]
public partial class Day15(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const char BORDER = '#';
    private const char ROBOT = '@';
    private const char BOX = 'O';
    private const char EMPTY = '.';

    private const char BOXL = '[';
    private const char BOXR = ']';

    #endregion

    private Map2<char>? _map;
    private Map2<char>? _wideMap;

    private int[] _path = [];    

    public void Init()
    {
        InitMap();
        InitWideMap();
    }

    public string SolvePart1()
        => SumOfGPS(_map!.Copy(), BOX).ToString();

    public string SolvePart2()
        => SumOfGPS(_wideMap!.Copy(), BOXL).ToString();

    #region Private methods

    private int SumOfGPS(Map2<char> map, char target)
    {
        var currentLocation = Array.IndexOf(map.Data, ROBOT);
        _path.ForEach(d => currentLocation = CheckAndMove(map, currentLocation, d));

        return map.WithIndex()
            .Where(c => c.item == target)
            .Sum(c => Distance(map, c.index));
    }

    private static int CheckAndMove(Map2<char> map, int location, int ddx)
    {
        var next = location + map.Directions[ddx];
        if (map[location] == BORDER || !Check(map, location, ddx, []))
        {
            return location;
        }
        else
        {
            Move(map, location, ddx, []);
            return next;
        }
    }

    private static bool Check(Map2<char> map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return true;

        visited.Add(location);
        var next = location + map.Directions[ddx];
        return map[location] switch
        {
            BORDER => false,
            EMPTY => true,
            BOXL => Check(map, next, ddx, visited) && Check(map, location + 1, ddx, visited),
            BOXR => Check(map, next, ddx, visited) && Check(map, location - 1, ddx, visited),
            _ => Check(map, next, ddx, visited)
        };
    }

    private static void Move(Map2<char> map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return;

        visited.Add(location);
        var next = location + map.Directions[ddx];
        switch (map[location])
        {
            case BOX:
            case ROBOT:
                Move(map, next, ddx, visited);
                break;
            case BOXL:
                Move(map, next, ddx, visited);
                Move(map, location + 1, ddx, visited);
                break;
            case BOXR:
                Move(map, next, ddx, visited);
                Move(map, location - 1, ddx, visited);
                break;
            default:
                return;
        }

        (map[next], map[location]) = (map[location], map[next]);
    }

    private void InitMap()
    {
        var sx = input.Lines.First().Length;
        _map = new
        (
            data: [..input.Lines
                .TakeWhile(line => line.Length > 0 && line.First() == BORDER)
                .SelectMany(line => line)],
            columns: sx
        );
        
        _path = input.Lines.Skip(_map.Rows)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
    }

    private void InitWideMap()
        => _wideMap = new(_map!.SelectMany(WideChar).ToArray(), 2 * _map!.Columns);

    private static IEnumerable<char> WideChar(char symbol) => symbol switch
    {
        BORDER => [BORDER, BORDER],
        BOX => [BOXL, BOXR],
        EMPTY => [EMPTY, EMPTY],
        ROBOT => [ROBOT, EMPTY],
        _ => throw new NotImplementedException()
    };

    private static int Ch2D(char symbol) => symbol switch
    {
        '^' => 0,
        '>' => 1,
        'v' => 2,
        '<' => 3,
        _ => throw new NotImplementedException()
    };    

    private static int Distance<T>(Map2<T> map, int location) 
    {
        var (x, y) = map.D1toD2(location);
        return y * 100 + x;
    }

    #endregion
}
