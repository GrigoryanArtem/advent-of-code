namespace Puzzles.Runner._2024;

[Puzzle("Code Chronicle", 25, 2024)]
public partial class Day25(ILinesInputReader input) : IPuzzleSolver
{
    private const int WIDTH = 5;
    private const char FILL = '#';

    private int[][] _locks = [];
    private int[][] _keys = [];

    public void Init()
    {
        List<int[]> keys = [];
        List<int[]> locks = [];

        var temp = new int[WIDTH];
        bool? isKey = null;

        for(int i = 0;i <= input.Lines.Length; i++)
        {
            if (i == input.Lines.Length || String.IsNullOrWhiteSpace(input.Lines[i]))
            {                
                if (isKey!.Value)
                {
                    keys.Add([.. temp]);
                }
                else
                {
                    locks.Add([.. temp]);
                }

                Array.Fill(temp, 0);                
                isKey = null;

                continue;
            } 

            input.Lines[i].Select(c => c == FILL ? 1 : 0)
                .WithIndex()
                .ForEach(d => temp[d.index] += d.item);

            isKey ??= temp.Sum() > 0;
        }

        _keys = [.. keys];
        _locks = [.. locks];
    }

    public string SolvePart1()
        => _keys.AsParallel().Sum(k => _locks.Count(lk => IsFit(k, lk))).ToString();

    public static bool IsFit(int[] a, int[] b)
        => a.Zip(b, (ai, bi) => ai + bi).All(d => d <= 7);
}
