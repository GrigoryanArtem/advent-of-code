namespace Puzzles.Runner._2024;

[Puzzle("Warehouse Woes", 15, 2024)]
public partial class Day15(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private const char ROBOT = '@';
    private const char BOX = 'O';
    private const char EMPTY = '.';    

    private const char BOXL = '[';
    private const char BOXR = ']';



    private int[] _path = [];
    private char[] _map = [];
    private char[] _widemap = [];

    private int[] _directions = [];
    private int[] _wdirections = [];

    private int _startLocation = 0;
    private int _wideStartLocation = 0;

    private int _sx = 0;
    private int _wsx = 0;

    public void Init()
    {
        InitMap();
        InitWideMap();
    }

    public string SolvePart1()
    {
        var map = new char[_map.Length];
        Array.Copy(_map, map, map.Length);
        var currentLocation = _startLocation;

        DrawMap(map, _sx);

        foreach (var i in _path)
        {
            currentLocation = SimpleMove(map, currentLocation, i) ?
                currentLocation + _directions[i] : currentLocation;
        }

        DrawMap(map, _sx);

        var answer = map.WithIndex()
            .Where(c => c.item == BOX)
            .Sum(c => Distance(c.index, _sx));

        return answer.ToString();
    }

    public string SolvePart2()
    {
        var map = new char[_widemap.Length];
        Array.Copy(_widemap, map, map.Length);
        var currentLocation = _wideStartLocation;

        DrawMap(map, _wsx);

        foreach (var (didx, idx) in _path.WithIndex())
        {
            currentLocation = CheckAndMove(map, currentLocation, didx, _wdirections);
            
        }

        DrawMap(map, _wsx);

        var answer = map.WithIndex()
            .Where(c => c.item == BOXL)
            .Sum(c => Distance(c.index, _wsx));

        return answer.ToString();
    }

    private bool SimpleMove(char[] map, int location, int ddx)
    {
        if (map[location] == BORDER)
            return false;

        if (map[location] == EMPTY)
            return true;

        var next = location + _directions[ddx];
        if(SimpleMove(map, next, ddx))
        {
            (map[next], map[location]) = (map[location], map[next]);
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool Check(char[] map, int location, int ddx, int[] directions, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return true;

        visited.Add(location);
        var next = location + directions[ddx];
        return map[location] switch
        {
            BORDER => false,
            EMPTY => true,
            BOXL => Check(map, next, ddx, directions, visited) && Check(map, location + 1, ddx, directions, visited),
            BOXR => Check(map, next, ddx, directions, visited) && Check(map, location - 1, ddx, directions, visited),
            _ => Check(map, next, ddx, directions, visited)
        };
    }

    private int CheckAndMove(char[] map, int location, int ddx, int[] directions)
    {        
        var next = location + directions[ddx];
        if (map[location] == BORDER || !Check(map, location, ddx, directions, []))
        {
            return location;            
        }
        else
        {
            Move(map, location, ddx, directions, []);
            return next;
        }
    }

    private void Move(char[] map, int location, int ddx, int[] directions, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return;

        visited.Add(location);
        var next = location + directions[ddx];
        switch (map[location])
        {
            case BOX:
            case ROBOT:
                Move(map, next, ddx, directions, visited);
                break;
            case BOXL:
                Move(map, next, ddx, directions, visited);
                Move(map, location + 1, ddx, directions, visited);
                break;
            case BOXR:
                Move(map, next, ddx, directions, visited);
                Move(map, location - 1, ddx, directions, visited);
                break;
            default:
                return;
        }

        (map[next], map[location]) = (map[location], map[next]);
    }

    #region Private methods

    private void InitMap()
    {
        _sx = input.Lines.First().Length;
        _map = [..input.Lines
            .TakeWhile(line => line.Length > 0 && line.First() == BORDER)
            .SelectMany(line => line)];

        var sy = _map.Length / _sx;

        _directions = [-_sx, 1, _sx, -1];
        _startLocation = Array.IndexOf(_map, ROBOT);
        _path = input.Lines.Skip(sy)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
    }

    private void InitWideMap()
    {
        _wsx = 2 * _sx;
        _widemap = _map.SelectMany(ChWC).ToArray();
        _wideStartLocation = Array.IndexOf(_widemap, ROBOT);
        _wdirections = [-_wsx, 1, _wsx, -1];
    }

    private IEnumerable<char> ChWC(char symbol) => symbol switch
    {
        BORDER => [BORDER, BORDER],
        BOX => [BOXL, BOXR],
        EMPTY => [EMPTY, EMPTY],
        ROBOT => [ROBOT, EMPTY],
        _ => throw new NotImplementedException()
    };

    private int Ch2D(char symbol) => symbol switch
    {
        '^' => 0,
        '>' => 1,
        'v' => 2,
        '<' => 3,
        _ => throw new NotImplementedException()
    };

    private int Mat2Vec(int x, int y)
        => y * _sx + x;

    private (int x, int y) Vec2Mat(int loc, int sx)
        => (loc % sx, loc / sx);

    private int Distance(int loc, int sx) 
    {
        var coord = Vec2Mat(loc, sx);
        return coord.y * 100 + coord.x;
    }

    private static void DrawMap(char[] map, int sx)
    {
        Console.WriteLine();

        for(int i = 0; i < map.Length; i += sx)
        {
            for(int k = i; k < i + sx; k++)
            {
                Console.Write(map[k]);
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }

    #endregion
}
