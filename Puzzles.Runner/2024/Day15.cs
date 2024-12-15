namespace Puzzles.Runner._2024;

[Puzzle("Warehouse Woes", 15, 2024)]
public partial class Day15(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private const char ROBOT = '@';
    private const char BOX = 'O';
    private const char EMPTY = '.';

    private int[] _path = [];
    private char[] _map = [];
    private int[] _directions = [];

    private int _startLocation = 0;
    private int _sx = 0;

    public void Init()
    {
        InitMap();
    }

    public string SolvePart1()
    {
        foreach (var i in _path)
        {
            _startLocation = Move(_startLocation, i) ?
                _startLocation + _directions[i] : _startLocation;
        }

        var answer = _map.WithIndex()
            .Where(c => c.item == BOX)
            .Sum(c => D(c.index));

        return answer.ToString();
    }

    private bool Move(int location, int ddx)
    {
        if (_map[location] == BORDER)
            return false;

        if (_map[location] == EMPTY)
            return true;

        var next = location + _directions[ddx];
        if(Move(next, ddx))
        {
            (_map[next], _map[location]) = (_map[location], _map[next]);
            return true;
        }
        else
        {
            return false;
        }
    }

    #region Private methods

    private void InitMap()
    {
        _sx = input.Lines.First().Length;
        _map = [..input.Lines
            .TakeWhile(line => line.Length > 0 && line.First() == BORDER)
            .SelectMany(line => line)];

        var sy = _map.Length / _sx;

        _directions = [-sy, 1, sy, -1];
        _startLocation = Array.IndexOf(_map, ROBOT);
        _path = input.Lines.Skip(sy)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
    }

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

    private (int x, int y) Vec2Mat(int loc)
        => (loc % _sx, loc / _sx);

    private int D(int loc) 
    {
        var coord = Vec2Mat(loc);
        return coord.y * 100 + coord.x;
    }

    #endregion
}
