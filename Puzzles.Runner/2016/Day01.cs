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

    private (bool isLeft, int distance)[] _instructions = [];

    public void Init()
        => _instructions = [.. input.Text
            .Split(", ", StringSplitOptions.RemoveEmptyEntries)
            .Select(s => (s[0] == 'L', Convert.ToInt32(s[1..])))];

    public string SolvePart1()
    {
        var pos = Vec2.Zero;
        var didx = 0;

        foreach (var instr in _instructions)
        {
            didx = (didx + (instr.isLeft ? 1 : 3)) & 3;            
            pos += DIRS[didx] * instr.distance;
        }

        return AOC.ManhattanDistance(Vec2.Zero, pos).ToString();
    }

    public string SolvePart2()
    {
        var pos = Vec2.Zero;
        var didx = 0;


        HashSet<Vec2> positions = [pos];
        foreach (var instr in _instructions)
        {
            didx = (didx + (instr.isLeft ? 1 : 3)) & 3;
            var dir = DIRS[didx];

            for (int i = 0; i < instr.distance; i++)
            {
                pos += dir;

                if (positions.Contains(pos))
                    return AOC.ManhattanDistance(Vec2.Zero, pos).ToString();

                positions.Add(pos);
            }
        }

        return AOC.ManhattanDistance(Vec2.Zero, pos).ToString();
    }
}
