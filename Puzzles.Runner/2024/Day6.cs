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

    private const int NUMBER_OF_TASKS = 128;

    #endregion

    #region Members

    private int[] _directions = [];
    private byte[] _map = [];

    private int _location = 0;
    private int _sx = 0;

    #endregion

    public void Init()
    {
        _sx = input.Lines.Length + 2;
        var sy = input.Lines.First().Length + 2;

        _map = new byte[_sx * sy];

        for (int x = 0; x < _sx; x++)
            _map[Mat2Arr(x, 0)] = _map[Mat2Arr(x, sy - 1)] = BORDER;

        for (int y = 0; y < sy; y++)
            _map[Mat2Arr(0, y)] = _map[Mat2Arr(_sx - 1, y)] = BORDER;

        for (int y = 0; y < input.Lines.Length; y++)
        {
            for (int x = 0; x < input.Lines[y].Length; x++)
            {
                if (input.Lines[y][x] == OBSTRUCTION_SYMBOL)
                    _map[Mat2Arr(x + 1, y + 1)] = OBSTRUCTION;

                if (input.Lines[y][x] == GUARD_SYMBOL)
                    _location = Mat2Arr(x + 1, y + 1);
            }
        }

        _directions = [-sy, 1, sy, -1];
    }

    public string SolvePart1()
        => FindPath(_location, START_DIRECTION, new byte[_map.Length]).ToString();

    public string SolvePart2()
    {
        var chunkSize = _map.Length / NUMBER_OF_TASKS;
        var tasks = Enumerable.Range(0, NUMBER_OF_TASKS)
            .Select(i => BrutForceAsync(i * chunkSize, chunkSize))
            .ToArray();

        Task.WaitAll(tasks);

        return tasks.Sum(t => t.Result).ToString();
    }

    #region Private methods

    private Task<int> BrutForceAsync(int start, int count)
        => Task.Run(() => BruteForce(start, count));

    private int BruteForce(int start, int count)
    {
        var buffer = new byte[_map.Length];
        var end = start + count;

        int sum = 0;
        for (int i = start; i < end && i < _map.Length; i++)
        {
            Array.Clear(buffer);
            if (_map[i] == EMPTY && FindPath(_location, START_DIRECTION, buffer, i) == HAS_LOOP)
                sum++;
        }

        return sum;
    }

    private int FindPath(int location, int direction, byte[] buffer, int? obstructionLocation = null)
    {
        while (_map[location] != BORDER)
        {
            var de = Dir2Flg(direction);
            if ((buffer[location] & de) == de)
                return HAS_LOOP;

            buffer[location] |= de;

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

        return buffer.Count(d => d > 0);
    }

    private static byte Dir2Flg(int direction)
        => (byte)(1 << direction);

    private int Mat2Arr(int x, int y)
        => y * _sx + x;

    #endregion
}
