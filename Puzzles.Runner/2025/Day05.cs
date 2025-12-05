namespace Puzzles.Runner._2025;

[Puzzle("Cafeteria", 5, 2025)]
internal class Day05(IFullInputReader input) : IPuzzleSolver
{
    private record Range(ulong Start, ulong End) 
    {
        public bool Contains(ulong value)
            => value >= Start && value <= End;

        public bool Overlaps(Range range) 
            => !(range.End < Start || range.Start > End);

        public Range Merge(Range range) 
            => new(Math.Min(Start, range.Start), Math.Max(End, range.End));

        public ulong Length() 
            => End - Start + 1;
    }

    private Range[] _ranges = [];
    private ulong[] _ingredients = [];

    public void Init()
    {
        var parts = input.Text.Split("\r\n\r\n", 2);

        _ranges = [.. parts[0].Split(Environment.NewLine).Select(line =>
        {
            var tokens = line.Split('-').Select(UInt64.Parse).ToArray();
            return new Range(tokens[0], tokens[1]);
        })];

        _ingredients = [.. parts[1].Split(Environment.NewLine).Select(UInt64.Parse)];
    }

    public string SolvePart1()
        => _ingredients.Count(i => _ranges.Any(r => r.Contains(i))).ToString();

    public string SolvePart2()
    {
        var merged = new Stack<Range>();

        foreach (var range in _ranges.OrderBy(r => r.Start))
        {
            if (!merged.TryPeek(out var peek) || !peek.Overlaps(range))
            {
                merged.Push(range);
            }
            else
            {
                var last = merged.Pop();
                merged.Push(last.Merge(range));
            }
        }

        return merged.UInt64Sum(r => r.Length()).ToString();
    }
}
