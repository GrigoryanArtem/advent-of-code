namespace Puzzles.Runner._2022;

[Puzzle("Camp Cleanup", 4, 2022)]
public class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private record Range(int Start, int End);
    private (Range r1, Range r2)[] _ranges = [];

    public void Init() 
        => _ranges = [..input.GetTokens(",", T2R).Select(t => Sort(t.First(), t.Last()))];    

    public string SolvePart1()
        => _ranges.Count(t => Contains(t.r1, t.r2)).ToString();
    
    public string SolvePart2()
        => _ranges.Count(t => Overlap(t.r1, t.r2)).ToString();

    private static bool Contains(Range r1, Range r2)
        => r1.Start <= r2.Start && r1.End >= r2.End;
    
    private static bool Overlap(Range r1, Range r2)
        => r2.Start >= r1.Start && r2.Start <= r1.End ||
            r2.End >= r1.Start && r2.End <= r1.End;

    private static (Range r1, Range r2) Sort(Range r1, Range r2)
    {
        if ((r2.End - r2.Start) > (r1.End - r1.Start))
            (r1, r2) = (r2, r1);

        return (r1, r2);
    }

    private static Range T2R(string token)
    {
        var parts = token.Split("-", 2).Select(Int32.Parse).ToArray();
        return new(parts[0], parts[1]);
    }
}
