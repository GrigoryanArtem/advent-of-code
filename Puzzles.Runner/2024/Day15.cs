using Map = Puzzles.Base.Entites.Map2<char>;

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

    private Map? _map;
    private Map? _wideMap;

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

    private int SumOfGPS(Map map, char target)
    {
        var currentLocation = Array.IndexOf(map.Data, ROBOT);
        _path.ForEach(d => currentLocation = TryMove(map, currentLocation, d));

        return map.WithIndex()
            .Where(c => c.item == target)
            .Sum(c => Distance(map, c.index));
    }

    private static int TryMove(Map map, int location, int ddx)
    {
        var next = map.Next(location, ddx);
        if (map[location] == BORDER || !CanMove(map, location, ddx, []))
        {
            return location;
        }
        else
        {
            Move(map, location, ddx, []);
            return next;
        }
    }

    private static bool CanMove(Map map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return true;

        visited.Add(location);
        var next = map.Next(location, ddx);
        return map[location] switch
        {
            BORDER => false,
            EMPTY => true,
            BOXL => CanMove(map, next, ddx, visited) && CanMove(map, map.Next(location, Map.RIGHT), ddx, visited),
            BOXR => CanMove(map, next, ddx, visited) && CanMove(map, map.Next(location, Map.LEFT), ddx, visited),
            _ => CanMove(map, next, ddx, visited)
        };
    }

    private static void Move(Map map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return;

        visited.Add(location);
        var next = map.Next(location, ddx);
        switch (map[location])
        {
            case BOX:
            case ROBOT:
                Move(map, next, ddx, visited);
                break;
            case BOXL:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map.RIGHT), ddx, visited);
                break;
            case BOXR:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map.LEFT), ddx, visited);
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

    private static int Distance(Map map, int location) 
    {
        var (x, y) = map.D1toD2(location);
        return y * 100 + x;
    }

    #endregion
}
