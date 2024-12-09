namespace Puzzles.Runner._2024;

[Puzzle("Guard Gallivant", 6, 2024)]
public class Day6(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const int NO_JUMP = -1;
    private const int NO_OBSTRUCTION = -1;

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

    private int[] _jumps = [];

    private bool[][] _pathBuffers = [];    
    private int[][] _jumpsBuffers = [];    

    #endregion

    public void Init()
    {
        _sx = input.Lines.First().Length + 2;
        var sy = input.Lines.Length + 2;

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
        CalculateJumps(); 

        _pathBuffers = new bool[NUMBER_OF_TASKS][];
        _jumpsBuffers = new int[NUMBER_OF_TASKS][];

        for (int i = 0; i < NUMBER_OF_TASKS; i++)
        {
            _pathBuffers[i] = new bool[_jumps.Length];
            _jumpsBuffers[i] = new int[_jumps.Length];

            Array.Copy(_jumps, _jumpsBuffers[i], _jumps.Length);
        }
    }

    public string SolvePart1()
    {
        var buffer = _pathBuffers.First();
        FindLoop(_location, buffer, _jumpsBuffers[0]).ToString();

        var sum = 1;
        for (var (i, success) = (0, false); i < buffer.Length; i += 4, success = false) 
        {
            for (int d = 0; d < _directions.Length && !success; d++)
                success |= buffer[i + d];

            if(success)
                sum++;
        }

        return sum.ToString();
    }

    public string SolvePart2()
    {
        var chunkSize = (int)Math.Ceiling((double)_map.Length / NUMBER_OF_TASKS);
        var tasks = Enumerable.Range(0, NUMBER_OF_TASKS)
            .Select(i => BrutForceAsync(i * chunkSize, chunkSize, _pathBuffers[i], _jumpsBuffers[i]))
            .ToArray();

        Task.WaitAll(tasks);

        return tasks.Sum(t => t.Result).ToString();
    }

    #region Private methods

    private Task<int> BrutForceAsync(int start, int count, bool[] pathBuffer, int[] jumpsBuffer)
        => Task.Run(() => BruteForce(start, count, pathBuffer, jumpsBuffer));

    private int BruteForce(int start, int count, bool[] pathBuffer, int[] jumpsBuffer)
    {
        var end = Math.Min(start + count, _map.Length);
        int sum = 0;        

        for (int i = start; i < end; i++)
        {
            if (_map[i] == EMPTY)
            {
                RecalculateJumps(jumpsBuffer, i, i);

                sum += FindLoop(_location, pathBuffer, jumpsBuffer) ? 1 : 0;

                RecalculateJumps(jumpsBuffer, i, NO_OBSTRUCTION);
            }                
        }

        return sum;
    }

    private bool FindLoop(int location, bool[] pathBuffer, int[] jumpsBuffer)
    {
        Array.Clear(pathBuffer);

        var jmp = location * _directions.Length;
        while (jumpsBuffer[jmp] != NO_JUMP)
        {
            if (pathBuffer[jmp])
                return true;

            pathBuffer[jmp] = true;            
            jmp = jumpsBuffer[jmp];
        }

        return false;
    }

    private int Mat2Vec(int x, int y)
        => y * _sx + x;

    private int Loc2Jmp(int location, int direction)
        => _directions.Length * location + direction;

    private void RecalculateJumps(int[] buffer, int loc, int obstruction)
    {
        if (_map[loc] == BORDER)
            return;

        for (int d = 0; d < _directions.Length; d++)
            CalculateJumps(buffer, loc + _directions[d], obstruction);
    }

    private void CalculateJumps(int[] buffer, int loc, int obstruction)
    {
        if (_map[loc] == BORDER)
            return;

        for (int d = 0; d < _directions.Length; d++)
        {
            var next = loc + _directions[d];
            var jmp = Loc2Jmp(loc, d);

            if (next == obstruction)
            {
                buffer[jmp] = Loc2Jmp(loc, (d + 1) % _directions.Length);
                continue;
            }

            buffer[jmp] = _map[next] switch
            {
                EMPTY => Loc2Jmp(next, d),
                OBSTRUCTION => Loc2Jmp(loc, (d + 1) % _directions.Length),
                _ => NO_JUMP
            };
        }
    }

    private void CalculateJumps()
    {
        _jumps = new int[_map.Length * _directions.Length];
        Array.Fill(_jumps, NO_JUMP);

        for (int loc = _sx; loc < _map.Length; loc++)
            CalculateJumps(_jumps, loc, NO_OBSTRUCTION);
    } 

    #endregion
}
