namespace Puzzles.Runner._2018;

[Puzzle("Inventory Management System", 2, 2018)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private readonly int[] _buffer = new int ['z' - 'a' + 1];

    public string SolvePart1()
    {
        var result = new int[2];
        var reps = new int[] { 2, 3 };

        input.Lines.ForEach(line => Check(line, reps, result));
        return result.Mul(x => x).ToString();
    }

    public string SolvePart2()
    {
        string? res = null;

        foreach (var line1 in input.Lines)
        {
            foreach (var line2 in input.Lines)
            {
                if (line1 == line2)
                    continue;

                var same = Common(line1, line2).ToArray();

                if (same.Length == line1.Length - 1)
                {
                    res = new string(same);
                    break;
                }
            }

            if (res is not null)
                break;
        }

        return res ?? "NO ANSWER";
    }

    public static IEnumerable<char> Common(string str1, string str2)
        => str1.Zip(str2, (c1, c2) => (c1, c2))
            .Where(d => d.c1 == d.c2)
            .Select(d => d.c1);

    public void Check(string str, int[] reps, int[] result)
    {
        Array.Clear(_buffer);

        foreach (var ch in str)         
            _buffer[ch - 'a']++;

        foreach (var (rep, idx) in reps.WithIndex())
            if (_buffer.Contains(rep))
                result[idx]++;
    }
}
