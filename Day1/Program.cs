const string INPUT = "input.txt";

List<int> first = [];
List<int> second = [];

Dictionary<int, uint> counter = [];

foreach (var line in File.ReadAllLines(INPUT))
{
    var tokens = line.Split([' '], StringSplitOptions.RemoveEmptyEntries)
        .Select(t => Convert.ToInt32(t.Trim()))
        .ToArray();

    var (f, s) = (tokens.First(), tokens.Last());

    first.Add(f);
    second.Add(s);

    counter.TryAdd(s, 0);
    counter[s]++;
}

first.Sort();
second.Sort();

var part1 = first.Zip(second, (f, s) => Math.Abs(s - f)).Sum();
var part2 = first.Aggregate(0UL, (acc, v) => acc += (uint)(v * counter.GetValueOrDefault(v, 0U)));

Console.WriteLine($"part 1: {part1}");
Console.WriteLine($"part 2: {part2}");