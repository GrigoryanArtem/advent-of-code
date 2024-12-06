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

    private byte[][] _buffers = [];

    #endregion

    public void Init()
    {
        _sx = input.Lines.Length + 2;
        var sy = input.Lines.First().Length + 2;

        _map = new byte[_sx * sy];

        for (int x = 0; x < _sx; x++)
            _map[Mat2Vec(x, 0)] = _map[Mat2Vec(x, sy - 1)] = BORDER;

        for (int y = 0; y < sy; y++)
            _map[Mat2Vec(0, y)] = _map[Mat2Vec(_sx - 1, y)] = BORDER;

        for (int y = 0; y < input.Lines.Length; y++)
        {
            for (int x = 0; x < input.Lines[y].Length; x++)
            {
                if (input.Lines[y][x] == OBSTRUCTION_SYMBOL)
                    _map[Mat2Vec(x + 1, y + 1)] = OBSTRUCTION;

                if (input.Lines[y][x] == GUARD_SYMBOL)
                    _location = Mat2Vec(x + 1, y + 1);
            }
        }

        _directions = [-sy, 1, sy, -1];

        _buffers = new byte[NUMBER_OF_TASKS][];
        for (int i = 0; i < NUMBER_OF_TASKS; i++)
            _buffers[i] = new byte[_map.Length];        
    }

    public string SolvePart1()
        => FindPath(_location, START_DIRECTION, _buffers[0]).ToString();

    public string SolvePart2()
    {
        var chunkSize = _map.Length / NUMBER_OF_TASKS;
        var tasks = Enumerable.Range(0, NUMBER_OF_TASKS)
            .Select(i => BrutForceAsync(i * chunkSize, chunkSize, _buffers[i]))
            .ToArray();

        Task.WaitAll(tasks);

        return tasks.Sum(t => t.Result).ToString();
    }

    #region Private methods

    private Task<int> BrutForceAsync(int start, int count, byte[] buffer)
        => Task.Run(() => BruteForce(start, count, buffer));

    private int BruteForce(int start, int count, byte[] buffer)
    {
        var end = Math.Min(start + count, _map.Length);
        int sum = 0;

        for (int i = start; i < end; i++)        
            if (_map[i] == EMPTY && FindLoop(_location, START_DIRECTION, buffer, i))
                sum++;

        return sum;
    }

    private bool FindLoop(int location, int direction, byte[] buffer, int obstructionLocation)
    {
        Array.Clear(buffer);

        while (_map[location] != BORDER)
        {
            var de = Dir2Flg(direction);
            if ((buffer[location] & de) == de)
                return true;

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

        return false;
    }

    private int FindPath(int location, int direction, byte[] buffer)
    {
        Array.Clear(buffer);

        while (_map[location] != BORDER)
        {
            var de = Dir2Flg(direction);
            if ((buffer[location] & de) == de)
                return HAS_LOOP;

            buffer[location] |= de;

            var next = location + _directions[direction];
            if (_map[next] == OBSTRUCTION)
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

    private int Mat2Vec(int x, int y)
        => y * _sx + x;

    #endregion
}
