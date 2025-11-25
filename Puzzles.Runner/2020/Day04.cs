using System.Text.RegularExpressions;

namespace Puzzles.Runner._2020;

using Passport = Dictionary<string, string>;

[Puzzle("Passport Processing", 4, 2020)]
public partial class Day043(IFullInputReader input) : IPuzzleSolver
{
    private static readonly Dictionary<string, Regex> RULES = new()
    {
        { "byr", new(@"^(19[2-9][0-9]|200[0-2])$") },
        { "iyr", new(@"^(201[0-9]|2020)$") },
        { "eyr", new(@"^(202[0-9]|2030)$") },
        { "hgt", new(@"^(1[5-8][0-9]cm|19[0-3]cm|59in|6[0-9]in|7[0-6]in)$") },
        { "hcl", new(@"^(#([0-9a-f]){6})$") },
        { "ecl", new(@"^(amb|blu|brn|gry|grn|hzl|oth)$") },
        { "pid", new(@"^([0-9]{9})$") }
    };

    private Passport[] _passports = [];


    public void Init()
    {
        _passports = [.. input.Text.Split("\r\n\r\n")
            .Select(line => PassportRegex()
                .Matches(line)
                .ToDictionary(m => m.Groups["name"].Value, m => m.Groups["value"].Value.Trim()))];
    }

    public string SolvePart1()
        => _passports.AsParallel().Count(p => RULES.All(rf => p.ContainsKey(rf.Key))).ToString();    

    public string SolvePart2()
        => _passports
                .Where((p => RULES.All(rf => p.ContainsKey(rf.Key))))
                .AsParallel()
                .Count(p => p.All(kv => 
                        RULES.ContainsKey(kv.Key) ? RULES[kv.Key].Match(kv.Value).Success : true))
                .ToString();
    

    [GeneratedRegex(@"(?<name>\S+)\:(?<value>\S+)")]
    private static partial Regex PassportRegex();
}
