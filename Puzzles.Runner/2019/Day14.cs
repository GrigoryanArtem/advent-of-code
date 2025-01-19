namespace Puzzles.Runner._2019;

[Puzzle("Space Stoichiometry", 14, 2019)]
public class Day14(ILinesInputReader input) : IPuzzleSolver
{
    private record Element(string Name, long Quantity);
    private record Reaction(Element[] Inputs, Element Output);

    private const string ORE = "ORE";
    private const string FUEL = "FUEL";
    private const long TRILLION = 1000000000000;

    private Dictionary<string, Reaction> _reactions = [];

    public void Init()
        => _reactions = input.Lines.Select(Str2Reaction).ToDictionary(r => r.Output.Name, r => r);

    public string SolvePart1()
    {
        var counts = new Dictionary<string, long>();
        var over = new Dictionary<string, long>();
        Count(FUEL, 1, counts, over);

        return counts[ORE].ToString();
    }

    public string SolvePart2()
    {
        var counts = new Dictionary<string, long>();
        var over = new Dictionary<string, long>();

        Count(FUEL, 1, counts, over);

        var count = TRILLION / counts[ORE];

        counts.Clear();
        over.Clear();

        var ores = TRILLION;
        Count(FUEL, count, counts, over);

        while(TRILLION > counts[ORE])
        {
            Count(FUEL, 1, counts, over);
            count++;
        }


        return (count - 1).ToString();
    }

    private void Count(string from, long count, Dictionary<string, long> counts, Dictionary<string, long> over)
    {
        var realCount = Math.Max(0, count - over.GetOrAdd(from, 0));
        over[from] -= (count - realCount);

        if (from == ORE || realCount == 0)
            return;

        var reaction = _reactions[from];

        var reps = (long)Math.Ceiling((double)realCount / reaction.Output.Quantity);
        var output = reps * reaction.Output.Quantity;

        over.TryAdd(from, 0);
        over[from] += output - realCount;

        foreach (var input in reaction.Inputs)
        {
            var needed = reps * input.Quantity;

            counts.TryAdd(input.Name, 0);
            counts[input.Name] += needed;

            Count(input.Name, needed, counts, over);
        }
    }

    private static Reaction Str2Reaction(string line)
    {
        var tokens = line.Split("=>", StringSplitOptions.RemoveEmptyEntries);
        var input = tokens[0].Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(Str2El)
            .ToArray();

        return new(input, Str2El(tokens[1]));
    }

    private static Element Str2El(string str)
    {
        var tokens = str.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new(tokens[1], long.Parse(tokens[0]));
    }
}
