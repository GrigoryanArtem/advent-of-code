namespace Puzzles.Runner._2015;

[Puzzle("Doesn't He Have Intern-Elves For This?", 5, 2015)]
public class Day05(ILinesInputReader input) : IPuzzleSolver
{    
    private static readonly string[] _banned = ["ab", "cd", "pq", "xy"];

    public string SolvePart1()
        => input.Lines.Count(IsValid1).ToString();

    public string SolvePart2()
        => input.Lines.Count(line => IsValid2(line)).ToString();

    public static bool IsValid1(string str)
    {
        var vowels = str.Count(c => c is 'a' or 'e' or 'i' or 'o' or 'u');

        if (vowels < 3)
            return false;

        var doubles = false;
        for (int i = 1; !doubles && i < str.Length; i++)
            doubles |= str[i] == str[i - 1];

        if (!doubles)
            return false;

        return !_banned.Any(str.Contains);
    }

    public static bool IsValid2(ReadOnlySpan<char> str)
    {
        var reps = false;
        var pairs = false;

        for (int i = 2; i < str.Length; i++)
        {
            reps |= str[i] == str[i - 2];

            for (int k = i; !pairs && k < str.Length - 1; k++)
                pairs |= (str[k] == str[i - 2]) && (str[k + 1] == str[i - 1]);
        }

        return reps && pairs;
    }
}
