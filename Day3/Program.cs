using System.Text.RegularExpressions;

const string INPUT = "input.txt";
const string EXAMPLE = "example.txt";

Regex regex = MulRegex();
Regex doRegex = DoRegex();

var text = File.ReadAllText(INPUT);

var mul = regex.Matches(text);
var @do = doRegex.Matches(text);

var part1 = mul.Aggregate(0L, (acc, m) => acc + (GetInt(m, "a") * GetInt(m, "b")));
var part2 = Calculate(mul.Concat(@do).OrderBy(m => m.Index));

Console.WriteLine($"part 1: {part1}");
Console.WriteLine($"part 2: {part2}");

int GetInt(Match m, string group )
    => Convert.ToInt32(m.Groups[group].Value);

long Calculate(IEnumerable<Match> matches)
{
    bool enable = true;
    long sum = 0;

    foreach(var m in matches)
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

partial class Program
{
    [GeneratedRegex(@"(?<do>do)(?<not>n't)?\(\)", RegexOptions.Compiled)]
    private static partial Regex DoRegex();

    [GeneratedRegex(@"mul\((?<a>\d+)\,(?<b>\d+)\)", RegexOptions.Compiled)]
    private static partial Regex MulRegex();
}