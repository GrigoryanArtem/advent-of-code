namespace Puzzles.Runner._2018;

using Puzzles.Runner.Base;
using System.Collections;
using Mat = Mat2<int>;

[Puzzle("Chronal Coordinates", 6, 2018)]
public partial class Day06(
    ILinesInputReader input,
    IRunInfo runInfo) : IPuzzleSolver
{
    private readonly int _threshold = runInfo.IsExample ? 32 : 10000;
    private Vec2[] _positions = [];
    public void Init()
    {
        _positions = [.. input.GetTokens(", ", Int32.Parse).Select(ia => new Vec2(ia[0], ia[1]))];
    }

    public string SolvePart1()
    {        
        var mat = CreateMat();
        mat.FillBorders(-2);
        
        var inf = new BitArray(_positions.Length + 1);

        Simulate(mat, inf);        

        var max = mat.Where(id => id > 0 && !inf[id]).GroupBy(d => d).Max(g => g.Count());


        return max.ToString();
    }

    public string SolvePart2()
    {
        var sx = _positions.Max(d => d.X) + 2;
        var sy = _positions.Max(d => d.Y) + 1;

        var area = 0;

        Parallel.For(0, sx, x =>
        {
            for (int y = 0; y < sy; y++)
            {
                var sum = _positions.Sum(v => AOC.ManhattanDistance(x, y, v.X, v.Y));

                if (sum < _threshold)
                    Interlocked.Increment(ref area);                    
            }
        });

        return area.ToString();
    }

    private void Simulate(Mat mat, BitArray inf)
    {
        var steps = mat.CreateBuffer(0);

        var queue = new Queue<int>();
        foreach (var (pos, idx) in _positions.WithIndex())
        {
            mat[pos.X, pos.Y] = idx + 1;
            queue.Enqueue(mat.D2toD1(pos.X, pos.Y));
        }

        while (queue.TryDequeue(out var pos))
        {
            var id = mat[pos];

            if (id < 0)
                continue;

            var step = steps[pos] + 1;

            foreach (var dir in mat.Directions)
            {
                var next = pos + dir;
                var nextValue = mat[next];

                if (nextValue == -2)
                {
                    inf[id] = true;
                }
                else if (nextValue == 0)
                {
                    mat[next] = id;
                    steps[next] = step;

                    queue.Enqueue(next);
                }
                else if (steps[next] == step && nextValue != id)
                {
                    mat[next] = -1;
                }
            }
        }
    }

    private Mat CreateMat()
    {
        var sx = _positions.Max(d => d.X);
        var sy = _positions.Max(d => d.Y);

        return Mat.Empty(sx + 2, sy + 2);
    }
}
