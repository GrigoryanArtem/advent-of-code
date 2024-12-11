using System.Text.RegularExpressions;

namespace Puzzles._2024;

[Puzzle("Mull It Over", 3, 2024)]
public partial class Day03(IFullInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
    {
        var matches = MulRegex().Matches(input.Text);
        return matches.Aggregate(0L, (acc, m) => acc + (GetInt(m, "a") * GetInt(m, "b"))).ToString();
    }

    public string SolvePart2()
    {
        var mulMatches = MulRegex().Matches(input.Text);
        var doMatches = DoRegex().Matches(input.Text);

        return Calculate(mulMatches.Concat(doMatches).OrderBy(m => m.Index)).ToString();
    }

    #region Additional 

    private static int GetInt(Match m, string group)
        => Convert.ToInt32(m.Groups[group].Value);

    private static long Calculate(IEnumerable<Match> matches)
    {
        bool enable = true;
        long sum = 0;

        foreach (var m in matches)
        {
            if (m.Groups["not"].Success)
            {
                enable = false;
            }
            else if (m.Groups["do"].Success)
            {
                enable = true;
            }
            else if (enable)
            {
                sum += GetInt(m, "a") * GetInt(m, "b");
            }
        }

        return sum;
    }

    #endregion

    #region Regex

    [GeneratedRegex(@"(?<do>do)(?<not>n't)?\(\)", RegexOptions.Compiled)]
    private static partial Regex DoRegex();

    [GeneratedRegex(@"mul\((?<a>\d+)\,(?<b>\d+)\)", RegexOptions.Compiled)]
    private static partial Regex MulRegex();

    #endregion
}

