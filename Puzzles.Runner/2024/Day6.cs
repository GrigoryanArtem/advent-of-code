namespace Puzzles.Runner._2024;

[Puzzle("Guard Gallivant", 6, 2024)]
public class Day6(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const int START_DIRECTION = 0;
    private const int HAS_LOOP = -1;

    private const byte EMPTY = 0;
    private const byte OBSTRUCTION = 1;
    private const byte BORDER = 2;

    private const char OBSTRUCTION_SYMBOL = '#';
    private const char GUARD_SYMBOL = '^';

    #endregion  

    private int[] _directions = [];
    private byte[] _map = [];
    
    private int _location = 0;    

    private int _sx = 0;    

    public void Init()
    {
        _sx = input.Lines.Length + 2;
        var sy = input.Lines.First().Length + 2;

        _map = new byte[_sx * sy];

        for (int x = 0; x < _sx; x++)
            _map[M2A(x, 0)] = _map[M2A(x, sy - 1)] = BORDER;

        for (int y = 0; y < sy; y++)
            _map[M2A(0, y)] = _map[M2A(_sx - 1, y)] = BORDER;

        for (int y = 0; y < input.Lines.Length; y++)
        {
            for (int x = 0; x < input.Lines[y].Length; x++)
            {
                if (input.Lines[y][x] == OBSTRUCTION_SYMBOL)
                    _map[M2A(x + 1, y + 1)] = OBSTRUCTION;

                if (input.Lines[y][x] == GUARD_SYMBOL)
                    _location = M2A(x + 1, y + 1);
            }
        }

        _directions = [-sy, 1, sy, -1];
    }

    public string SolvePart1()
        => FindPath(_location, START_DIRECTION).ToString();

    public string SolvePart2()
    {
        int sum = 0;

        for(int i = 0; i < _map.Length; i++)        
            if(_map[i] == EMPTY && FindPath(_location, START_DIRECTION, i) == HAS_LOOP)
                sum++;
        

        return sum.ToString();
    }

    //public string SolvePart2()
    //    => Enumerable.Range(0, _map.Length).Count(i => _map[i] == EMPTY && 
    //        FindPath(_location, START_DIRECTION, i) == HAS_LOOP).ToString();

    private int FindPath(int location, int direction, int? obstructionLocation = null)
    {
        HashSet<(int location, int direction)> visited = [];

        while (_map[location] != BORDER)
        {
            var dl = (location, direction);
            if (visited.Contains(dl))
                return HAS_LOOP;

            visited.Add(dl);

            var next = location + _directions[direction];   
            if (_map[next] == OBSTRUCTION || next == obstructionLocation)
            {
                direction = (direction + 1) % _directions.Length;
            }
            else
            {
                location = next;
            }
        }

        return visited.DistinctBy(v => v.location).Count();
    }

    private int M2A(int x, int y)
        => y * _sx + x;
}
