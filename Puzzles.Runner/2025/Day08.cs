using Puzzles.Runner.Base;

namespace Puzzles.Runner._2025;

[Puzzle("Playground", 8, 2025)]
public class Day08(ILinesInputReader input, IRunInfo run) : IPuzzleSolver
{
    private readonly int ITERATIONS = run.IsExample ? 10 : 1000;

    private record struct Connection(int From, int To)
    {
        public readonly void Deconstruct(out int from, out int to)
            => (from, to) = (From, To);
    }

    private class Mct(int size)
    {
        private readonly int[] _buffer = new int[size];

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

        public int[] Sizes()
        {
            Tree.ForEach(a => _buffer[Get(a)]++);
            return _buffer;
        }
    }

    private Vec3[] _points = [];

    public void Init()
        => _points = [.. input.Lines.Select(line => new Vec3(line.Split(',').Select(Int32.Parse)))];

    public string SolvePart1()
    {
        var mct = FindMCT(ITERATIONS);
        return mct.Sizes()
            .OrderByDescending(x => x)
            .Take(3)
            .Mul(x => x)
            .ToString();
    }

    public string SolvePart2()
    {
        var mct = FindMCT(int.MaxValue);

        var (from, to) = mct.LastConnection;
        return ((long)_points[from].X * _points[to].X).ToString();
    }

    private Mct FindMCT(int iterations = -1)
    {
        var queue = PrepareQueue();
        var mct = new Mct(_points.Length);

        for (int i = 0; i < iterations && mct.Size > 1; i++)
            mct.Union(queue.Dequeue());

        return mct;
    }

    private PriorityQueue<Connection, double> PrepareQueue()
    {
        var queue = new PriorityQueue<Connection, double>();

        for (int i = 0; i < _points.Length; i++)
            for (int k = i + 1; k < _points.Length; k++)
                queue.Enqueue(new(i, k), AOC.SqrEuclideanDistance(_points[i], _points[k]));

        return queue;
    }
}
