using System.Text.RegularExpressions;

const string INPUT = "input.txt";
const string EXAMPLE = "example.txt";

Regex regex = new(@"mul\((?<a>\d+)\,(?<b>\d+)\)", RegexOptions.Compiled);

var text = File.ReadAllText(INPUT);
var part1 = regex.Matches(text).Aggregate(0L, (acc, m) => acc + (GetInt(m, "a") * GetInt(m, "b")));

Console.WriteLine($"part 1: {part1}");


// Console.WriteLine($"part 2: {part2}");


int GetInt(Match m, string group )
    => Convert.ToInt32(m.Groups[group].Value);


