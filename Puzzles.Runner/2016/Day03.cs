namespace Puzzles.Runner._2016;

using Mat = Mat2<int>;

[Puzzle("Squares With Three Sides", 3, 2016)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private Mat _inputMat = Mat.Null;

    public void Init()
    {
        _inputMat = new Mat([.. input.Lines.SelectMany(line => line
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(num => Int32.Parse(num)))], 3);
    }

    public string SolvePart1()
        => Enumerable.Range(0, _inputMat.Rows)
            .Count(row => IsValidTriangle(_inputMat.RowSpan(row)))
            .ToString();

    public string SolvePart2()
    {
        var count = 0;
        var sides = new int[3];

        for (int k = 0; k < _inputMat.Columns; k++)
            foreach (var chunk in Chunk(_inputMat.Column(k), sides))
                if (IsValidTriangle(sides))
                    count++;

        return count.ToString();
    }

    public static bool IsValidTriangle(Span<int> sides)
        => IsValidTriangle(sides[0], sides[1], sides[2]);

    private static bool IsValidTriangle(int a, int b, int c)
    {
        var max = AOC.Max3(a, b, c);
        return (a + b + c) - max > max;
    }

    private static IEnumerable<int[]> Chunk(IEnumerable<int> source, int[] buffer)
    {
        var column = source.GetEnumerator();
        var chunkSize = buffer.Length;

        var next = column.MoveNext();
        while (next)
        {
            for (int i = 0; i < chunkSize; i++)
            {
                buffer[i] = column.Current;
                next = column.MoveNext();
            }

            yield return buffer;
        }
    }
}
