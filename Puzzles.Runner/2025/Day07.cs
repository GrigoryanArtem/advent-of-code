namespace Puzzles.Runner._2025;

using Mat = Mat2<char>;
using Mat64 = Mat2<ulong>;

[Puzzle("Laboratories ", 7, 2025)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const char SPLITTER = '^';
    private const char START = 'S';

    private Mat _mat = Mat.Null;
    private Mat64 _sim = Mat64.Null;

    private int _startPos;

    public void Init()
    { 
        _mat = new Mat([.. input.Lines.SelectMany(line => line.ToCharArray())], input.Lines[0].Length);

        _sim = Mat64.Empty(_mat.Columns, _mat.Rows);
        _startPos = Array.IndexOf(_mat.Data, START);
    }
    
    public string SolvePart1()
        => Simulate(_mat, _startPos).ToString();
    public string SolvePart2()
    {
        var sim = SimulateTimeLines(_mat, _sim);
        return sim[_startPos].ToString();
    }
    
    private static int Simulate(Mat mat, int start)
    {
        var splitted = 0;

        var queue = new Queue<int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start);

        while(queue.TryDequeue(out var pos))
        {
            if (visited.Contains(pos) || pos >= mat.Data.Length)
                continue;

            visited.Add(pos);

            if (mat[pos] == SPLITTER)
            {
                queue.Enqueue(pos + mat.Directions[Mat.LEFT]);
                queue.Enqueue(pos + mat.Directions[Mat.RIGHT]);
                splitted++;
            }
            else
            {
                queue.Enqueue(pos + mat.Directions[Mat.DOWN]);
            }
        }

        return splitted;
    }

    public static Mat64 SimulateTimeLines(Mat mat, Mat64 sim)
    {
        var dd = mat.Directions[Mat.DOWN];
        var ld = mat.Directions[Mat.DOWN] + mat.Directions[Mat.LEFT];
        var rd = mat.Directions[Mat.DOWN] + mat.Directions[Mat.RIGHT];

        var lastRow = sim.RowSpan(mat.Rows - 1);
        for(int i = 0; i < lastRow.Length; i++)
            lastRow[i] = 1;

        for (int r = mat.Rows - 2; r >= 0; r--)
        {
            var start = mat.D2toD1(0, r);
            for(int i = 0; i < mat.Columns; i++)
            {
                var pos = start + i;
                sim[pos] = mat[pos] == SPLITTER 
                    ? sim[pos + ld] + sim[pos + rd]
                    : sim[pos + dd];
            }
        }

        return sim;
    }
}
