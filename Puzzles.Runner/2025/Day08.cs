using Puzzles.Runner.Base;

namespace Puzzles.Runner._2025;

[Puzzle("Playground", 8, 2025)]
public class Day08(ILinesInputReader input, IRunInfo run) : IPuzzleSolver
{
    private readonly int ITERATIONS = run.IsExample ? 10 : 1000;

    private record struct Connection(int From, int To, ulong SqrD) : IComparable<Connection>
    {
        public readonly int CompareTo(Connection other)
            => SqrD.CompareTo(other.SqrD);

        public readonly void Deconstruct(out int from, out int to)
            => (from, to) = (From, To);
    }

    private class UnionFind(int size)
    {        
        public int Size { get; private set; } = size;
        public int[] Tree { get; } = [.. Enumerable.Range(0, size)];

        public Connection LastConnection { get; private set; }

        public void Union(Connection connection)
        {
            int fp = Get(connection.From);
            int tp = Get(connection.To);

            if (fp != tp)
            {
                Tree[tp] = fp;
                LastConnection = connection;
                Size--;
            }
        }

        private int Get(int v)
        {
            if (Tree[v] == v)
                return v;

            return Tree[v] = Get(Tree[v]);
        }

        public int[] TopSizes(int n)
        {
            Span<int> sizes = stackalloc int[Tree.Length];

            foreach (var a in Tree)
                sizes[Get(a)]++;

            sizes.Sort();

            return sizes[^n..].ToArray();
        }
    }

    private Vec3[] _points = [];

    public void Init()
        => _points = [.. input.Lines.Select(line => new Vec3(line.Split(',').Select(Int32.Parse)))];

    public string SolvePart1()
    {
        var mct = RunUnionFind(ITERATIONS);
        return mct.TopSizes(3)
            .Mul(x => x)
            .ToString();
    }

    public string SolvePart2()
    {
        var mct = RunUnionFind(int.MaxValue);

        var (from, to) = mct.LastConnection;
        return ((long)_points[from].X * _points[to].X).ToString();
    }

    private UnionFind RunUnionFind(int iterations)
    {
        var queue = PrepareHeap();
        var mct = new UnionFind(_points.Length);

        for (int i = 0; i < iterations && mct.Size > 1; i++)
            mct.Union(queue.Pop());

        return mct;
    }

    private SpanHeap<Connection> PrepareHeap()
    {
        int n = _points.Length;
        int connections = n * (n - 1) / 2;
        
        var buffer = new Connection[connections].AsSpan();        

        int idx = 0;
        for (int i = 0; i < _points.Length; i++)
            for (int k = i + 1; k < _points.Length; k++)
                buffer[idx++] = new Connection(i, k, AOC.SqrEuclideanDistance(_points[i], _points[k]));
        
        return new SpanHeap<Connection>(buffer[..idx]);
    }
}
