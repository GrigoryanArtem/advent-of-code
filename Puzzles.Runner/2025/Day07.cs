namespace Puzzles.Runner._2025;

using Mat16 = Mat2<char>;
using Mat64 = Mat2<ulong>;

[Puzzle("Laboratories ", 7, 2025)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const char SPLITTER = '^';
    private const char START = 'S';

    private Mat16 _mat = Mat16.Null;
    private Mat64 _sim = Mat64.Null;
    private bool[] _visited = [];

    private int _startPos;

    public void Init()
    {
        _mat = new Mat16([.. input.Lines.SelectMany(line => line.ToCharArray())], input.Lines[0].Length);

        _sim = Mat64.Empty(_mat.Columns, _mat.Rows);
        _startPos = Array.IndexOf(_mat.Data, START);
        _visited = _mat.CreateBuffer<bool>();
    }

    public string SolvePart1()
        => Simulate(_mat, _startPos, _visited).ToString();

    public string SolvePart2()
    {
        var sim = SimulateTimeLines(_mat, _sim);
        return sim[_startPos].ToString();
    }

    private static int Simulate(Mat16 mat, int start, bool[] visited)
    {
        var splitted = 0;

        Array.Clear(visited);
        Span<int> stack = stackalloc int[1024];
        var sx = 0;

        stack[sx++] = start;

        while (sx > 0)
        {
            var pos = stack[--sx];
            if (pos >= mat.Data.Length || visited[pos])
                continue;

            visited[pos] = true;

            if (mat[pos] == SPLITTER)
            {
                stack[sx++] = pos + mat.Directions[Mat16.LEFT];
                stack[sx++] = pos + mat.Directions[Mat16.RIGHT];
                splitted++;
            }
            else
            {
                stack[sx++] = pos + mat.Directions[Mat16.DOWN];
            }
        }

        return splitted;
    }

    public static Mat64 SimulateTimeLines(Mat16 mat, Mat64 sim)
    {
        var dd = mat.Directions[Mat16.DOWN];
        var ld = mat.Directions[Mat16.DOWN] + mat.Directions[Mat16.LEFT];
        var rd = mat.Directions[Mat16.DOWN] + mat.Directions[Mat16.RIGHT];

        var lastRow = sim.RowSpan(mat.Rows - 1);
        for (int i = 0; i < lastRow.Length; i++)
            lastRow[i] = 1;

        for (int r = mat.Rows - 2; r >= 0; r--)
        {
            var start = mat.D2toD1(0, r);
            for (int i = 0; i < mat.Columns; i++)
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
