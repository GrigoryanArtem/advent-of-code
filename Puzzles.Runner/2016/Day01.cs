namespace Puzzles.Runner._2016;

[Puzzle("No Time for a Taxicab", 1, 2016)]
public class Day01(IFullInputReader input) : IPuzzleSolver
{
    private static readonly Vec2[] DIRS =
    [
        new(0,1),
        new(1,0),
        new(0,-1),
        new(-1,0)
    ];

    private (int offset, int distance)[] _instructions = [];

    public void Init()
        => _instructions = [.. input.Text
            .Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => (s[0] == 'L' ? 1 : 3, Convert.ToInt32(s[1..])))];

    public string SolvePart1()
        => AOC.ManhattanDistance(Vec2.Zero, Simulate().Last()).ToString();

    public string SolvePart2()
    {
        var positions = new HashSet<Vec2>();
        foreach (var pos in Simulate())
            if (!positions.Add(pos))
                return AOC.ManhattanDistance(Vec2.Zero, pos).ToString();

        return "NO ANSWER";
    }

    private IEnumerable<Vec2> Simulate()
    {
        var didx = 0;
        var pos = Vec2.Zero;

        yield return pos;

        foreach (var instr in _instructions)
        {
            didx = (didx + instr.offset) & 3;
            for (int i = 0; i < instr.distance; i++)
            {
                pos += DIRS[didx];
                yield return pos;
            }
        }
    }
}
