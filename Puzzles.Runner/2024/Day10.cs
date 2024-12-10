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

    public void Init()
        => InitMap();    

    public string SolvePart1()
        => _starts.Sum(i => GetTrailsCount(i, [])).ToString();

    public string SolvePart2()
        => _starts.Sum(i => GetStartRating(i, [])).ToString();

    #region Private methods

    private int GetTrailsCount(int location, HashSet<int> path)
    {
        if (_map[location] == BORDER || path.Contains(location))
            return 0;

        path.Add(location);

        if (_map[location] == TRAILHEAD)
            return 1;

        return _directions.Sum(d =>
        {
            var diff = _map[location + d] - _map[location];
            return diff == 1 ? GetTrailsCount(location + d, path) : 0;
        });
    }

    private int GetStartRating(int location, HashSet<int> path)
    {
        if (_map[location] == BORDER || path.Contains(location))
            return 0;

        if (_map[location] == TRAILHEAD)
            return 1;

        path.Add(location);

        var sum = _directions.Sum(d =>
        {
            var diff = _map[location + d] - _map[location];
            return diff == 1 ? GetStartRating(location + d, path) : 0;
        });

        path.Remove(location);

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
    }

    private int Mat2Vec(int x, int y)
        => y * _sizeX + x;

    private static byte C2B(char num)
        => (byte)(num - '0');

    #endregion
}
