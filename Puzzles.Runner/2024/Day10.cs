namespace Puzzles.Runner._2024;

[Puzzle("Hoof It", 10, 2024)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const byte BORDER = byte.MaxValue;
    private const byte START = 0;
    private const byte TRAILHEAD = 9;    

    #endregion

    private int[] _directions = [];

    private byte[] _map = [];
    private int[] _starts = [];
    private int _sizeX = 0;

    private Queue<int> _queue = [];
    private readonly HashSet<int> _path = new(10);

    public void Init()
        => InitMap();    

    public string SolvePart1()
        => _starts.Sum(i => GetTrails(i)).ToString();

    public string SolvePart2()
        => _starts.Sum(i => GetTrails(i, rating: true)).ToString();

    #region Private methods

    private int GetTrails(int location, bool rating = false)
    {
        _queue.Clear();
        _path.Clear();
        
        _queue.Enqueue(location);

        var sum = 0;

        while(_queue.Count > 0)
        {
            var current = _queue.Dequeue();

            if (_map[current] == BORDER || _path.Contains(current))
                continue;

            if (_map[current] == TRAILHEAD)
            {                
                if(!rating) 
                    _path.Add(current);

                sum++;
                continue;
            }

            Array.ForEach(_directions, d =>
            {
                if (_map[current + d] - _map[current] == 1)
                    _queue.Enqueue(current + d);
            });
        }

        return sum;
    }

    private void InitMap()
    {
        _sizeX = input.Lines.First().Length + 2;
        var sy = input.Lines.Length + 2;

        _map = new byte[_sizeX * sy];

        for (int x = 0; x < _sizeX; x++)
            _map[Mat2Vec(x, 0)] = _map[Mat2Vec(x, sy - 1)] = BORDER;

        for (int y = 0; y < sy; y++)
            _map[Mat2Vec(0, y)] = _map[Mat2Vec(_sizeX - 1, y)] = BORDER;

        for (int y = 0; y < input.Lines.Length; y++)
            for (int x = 0; x < input.Lines[y].Length; x++)
                _map[Mat2Vec(x + 1, y + 1)] = C2B(input.Lines[y][x]);

        _directions = [-sy, 1, sy, -1];
        _starts = _map.WithIndex()
            .Where(m => m.item == START)
            .Select(m => m.index)
            .ToArray();

        _queue = new Queue<int>(_map.Length);
    }

    private int Mat2Vec(int x, int y)
        => y * _sizeX + x;

    private static byte C2B(char num)
        => (byte)(num - '0');

    #endregion
}
