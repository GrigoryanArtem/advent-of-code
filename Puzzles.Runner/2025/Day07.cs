namespace Puzzles.Runner._2025;

using Mat = Mat2<char>;

[Puzzle("Laboratories ", 7, 2025)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const char SPLITTER = '^';
    private const char START = 'S';

    private Mat _mat = Mat.Null;
    private int _startPos;

    public void Init()
    { 
        _mat = new Mat([.. input.Lines.SelectMany(line => line.ToCharArray())], input.Lines[0].Length);
        _startPos = Array.IndexOf(_mat.Data, START);
    }
    
    public string SolvePart1()
        => Simulate(_mat, _startPos).ToString();
    public string SolvePart2()
        => CalculateTimeLines(_mat, _startPos, []).ToString();
    
    private static int Simulate(Mat mat, int start)
    {
        var splited = 0;

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
                splited++;
            }
            else
            {
                queue.Enqueue(pos + mat.Directions[Mat.DOWN]);
            }
        }

        return splited;
    }

    public static ulong CalculateTimeLines(Mat mat, int pos, Dictionary<int, ulong> mem)
    {
        while (pos < mat.Data.Length)
        {
            if (mem.TryGetValue(pos, out var saved))
                return saved;

            if (mat[pos] == SPLITTER)
            {
                return mem.AddAndReturn(pos, 
                    CalculateTimeLines(mat, pos + mat.Directions[Mat.LEFT], mem) + 
                    CalculateTimeLines(mat, pos + mat.Directions[Mat.RIGHT], mem));
            }
            else
            {
                pos += mat.Directions[Mat.DOWN];
            }
        }

        return 1UL;
    }
}
