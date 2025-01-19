namespace Puzzles.Runner._2019;

[Puzzle("Space Stoichiometry", 14, 2019)]
public class Day14(ILinesInputReader input) : IPuzzleSolver
{
    private record struct Element(int Id, long Quantity);
    private record struct Reaction(Element[] Inputs, Element Output);

    #region Constants

    private const int ORE = 0;
    private const int FUEL = 1;

    private const double ORE_COUNT = 1000000000000;

    #endregion

    private int _currentId = 2;
    private readonly Dictionary<string, int> _ids = new()
    {
        {"ORE", ORE},
        {"FUEL", FUEL}
    };

    private Reaction[] _reactions = [];

    public void Init()
    {
        var parse = input.Lines.Select(Str2Reaction).ToArray();
        _reactions = new Reaction[_currentId];
        parse.ForEach(r => _reactions[r.Output.Id] = r);
    }        

    public string SolvePart1()
        => Solve(FUEL, 1).ToString();

    public string SolvePart2()
    {
        var fuel = 1L;
        var next = 0L;

        while (fuel != next)
        {
            next = (int)(ORE_COUNT / Solve(FUEL, fuel) * fuel);
            (fuel, next) = (next, fuel);
        }

        return fuel.ToString();
    }

    #region Private methods

    private long Solve(int from, long count)
        => Solve(from, count, new long[_currentId], new long[_currentId]);

    private long Solve(int from, long count, long[] counts, long[] over)
    {
        var realCount = Math.Max(0, count - over[from]);
        over[from] -= (count - realCount);

        if (from == ORE || realCount == 0)
            return counts[ORE];

        var reaction = _reactions[from];

        var reps = (long)Math.Ceiling((double)realCount / reaction.Output.Quantity);
        var output = reps * reaction.Output.Quantity;

        over[from] += output - realCount;

        foreach (var input in reaction.Inputs)
        {
            var needed = reps * input.Quantity;
            counts[input.Id] += needed;

            Solve(input.Id, needed, counts, over);
        }

        return counts[ORE];
    }

    private Reaction Str2Reaction(string line)
    {
        var tokens = line.Split("=>", StringSplitOptions.RemoveEmptyEntries);
        var input = tokens[0].Split(",", StringSplitOptions.RemoveEmptyEntries)
            .Select(Str2Element)
            .ToArray();

        return new(input, Str2Element(tokens[1]));
    }

    private Element Str2Element(string str)
    {
        var tokens = str.Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);
        return new(GetId(tokens[1]), long.Parse(tokens[0]));
    }

    private int GetId(string name)
        => _ids.GetOrAdd(name, () => _currentId++);

    #endregion
}
