using System.Text.RegularExpressions;

namespace Puzzles.Runner._2020;

[Puzzle("Password Philosophy", 2, 2020)]
public partial class Day02(ILinesInputReader input) : IPuzzleSolver
{    
    private record Policy(int Min, int Max, char Symbol, string Password);

    private Policy[] _policies = [];

    public void Init() 
        => _policies = [.. input.Lines.Select(line => 
        {
            var match = PolicyRegex().Match(line);

            var min = Convert.ToInt32(match.Groups["min"].Value);
            var max = Convert.ToInt32(match.Groups["max"].Value);
            var symbol = Convert.ToChar(match.Groups["symbol"].Value);
            var password = match.Groups["pass"].Value;

            return new Policy(min, max, symbol, password);
        })];

    public string SolvePart1()
        => _policies
            .AsParallel()
            .Count(p =>
            {
                var charCount = p.Password.Count(c => c == p.Symbol);
                return charCount >= p.Min && charCount <= p.Max;
            })
            .ToString();

    public string SolvePart2()
         => _policies
            .AsParallel()
            .Count(p =>
            {
                var p1 = p.Password[p.Min - 1] == p.Symbol;
                var p2 = p.Password[p.Max - 1] == p.Symbol;

                return p1 ^ p2;
            })
            .ToString();

    [GeneratedRegex(@"^(?<min>\d+)-(?<max>\d+)\s+(?<symbol>\S+)\:\s+(?<pass>.*?)$")]
    private static partial Regex PolicyRegex();
}
