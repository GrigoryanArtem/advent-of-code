namespace Puzzles.Runner._2025;

[Puzzle("Cafeteria", 5, 2025)]
public class Day05(IFullInputReader input) : IPuzzleSolver
{
    private readonly record struct Range(ulong Start, ulong End) 
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

    private static readonly Comparer<Range> RANGE_COMPARER = 
        Comparer<Range>.Create((a, b) => a.Start.CompareTo(b.Start));

    private Range[] _ranges = [];
    private ulong[] _ingredients = [];

    public void Init()
    {
        var parts = input.Text.Split("\r\n\r\n", 2);

        _ranges = Merge(parts[0].Split(Environment.NewLine).Select(line =>
        {
            var tokens = line.Split('-').Select(UInt64.Parse).ToArray();
            return new Range(tokens[0], tokens[1]);
        }));

        _ingredients = [.. parts[1].Split(Environment.NewLine).Select(UInt64.Parse)];
    }

    public string SolvePart1()
        => _ingredients.Count(Contains).ToString();

    public string SolvePart2()
        => _ranges.UInt64Sum(r => r.Length()).ToString();    

    private bool Contains(ulong value)
    {        
        var idx = Array.BinarySearch(_ranges, new Range(value, value), RANGE_COMPARER);
        idx = idx < 0 ? ~idx - 1 : idx;

        return idx >= 0 && _ranges[idx].Contains(value);
    }        

    private static Range[] Merge(IEnumerable<Range> ranges)
    {
        Span<Range> merged = stackalloc Range[1024];
        var lidx = -1;

        foreach (var range in ranges.OrderBy(r => r.Start))
        {
            if (lidx < 0 || !merged[lidx].Overlaps(range))
            {
                merged[++lidx] = range;
            }
            else
            {                
                merged[lidx] = merged[lidx].Merge(range);
            }
        }

        return merged[..(lidx + 1)].ToArray();
    }
}
