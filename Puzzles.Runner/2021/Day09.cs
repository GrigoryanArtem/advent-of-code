namespace Puzzles.Runner._2021;

using Map = Mat2<byte>;

[Puzzle("Smoke Basin", 9, 2021)]
public class Day09(ILinesInputReader input) : IPuzzleSolver
{
    private Map _map = Map.Null;
    private Basin _bf = new(Map.Null);

    private class Basin(Map map)
    {        
        private readonly bool[] _buffer = map.CreateBuffer<bool>();

        public int GetSize(int start)
        {
            Array.Clear(_buffer, 0, _buffer.Length);
            Span<int> _stack = stackalloc int[4096];

            var ptr = 0;
            _stack[ptr++] = start;

            var size = 0;
            while (ptr > 0)
            {
                var current = _stack[--ptr];
                if (_buffer[current] || map[current] == 9)
                    continue;

                _buffer[current] = true;
                size++;

                foreach(var dir in map.Directions)
                    _stack[ptr++] = current + dir;
            }

            return size;
        }
    }

    public void Init()
    {
        _map = Map.WithBorders
        (
            data: [..input.Lines.SelectMany(line => line.AsEnumerable().Select(v => (byte)(v - '0')))],
            input.Lines[0].Length,
            9
        );

        _bf = new Basin(_map);
    }

    public string SolvePart1()
        => GetRiskPoints(_map)
            .Sum(r => r.val + 1)
            .ToString();
    
    public string SolvePart2()
        => GetRiskPoints(_map)
            .Select(r => _bf.GetSize(r.loc))
            .OrderByDescending(x => x)
            .Take(3)
            .Mul(x => x)
            .ToString();      

    public static IEnumerable<(int loc, int val)> GetRiskPoints(Map map)
    {
        for(int x = 1; x < map.Columns - 1; x++)
        {
            for(int y = 1; y < map.Rows - 1; y++)
            {
                var loc = map.D2toD1(x, y);
                var val = map[loc];

                var d = map.Directions;
                var lowPoint =
                    map[loc + d[0]] > val &&
                    map[loc + d[1]] > val &&
                    map[loc + d[2]] > val &&
                    map[loc + d[3]] > val;

                if (lowPoint)
                    yield return (loc, val);
            }
        }
    }
}
