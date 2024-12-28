namespace Puzzles.Runner._2019;

using Instruction = (int dir, int distance);

[Puzzle("Crossed Wires", 3, 2019)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private readonly Vec2[] _directions = 
    [
        new(0, 1),
        new(1, 0),
        new(0, -1),
        new(-1, 0),
    ];

    private Instruction[][] _paths = [];

    public void Init()
        => _paths = [.. input.GetTokens(",", str => (dir: DC2D(str[0]), distance: Convert.ToInt32(str[1..])))];
    
    public string SolvePart1()
        => Intersect(_paths[0], _paths[1], (p, _, _) => AOC.ManhattanDistance(Vec2.Zero, p)).Min().ToString();

    public string SolvePart2()
        => Intersect(_paths[0], _paths[1], (_, i1, i2) => i1 + i2).Min().ToString();

    private int[] Intersect(Instruction[] path1, Instruction[] path2, Func<Vec2, int, int, int> metric)
    {
        var data = new Dictionary<Vec2, int>();
        var result = new List<int>();

        var current = new Vec2(0, 0);
        int counter = 1;
        foreach (var (dir, distance) in path1)
        {
            for (int i = 0; i < distance; i++, counter++) 
            {
                current += _directions[dir];
                data.TryAdd(current, counter);
            }
        }

        current = new Vec2(0, 0);
        counter = 1;
        foreach (var (dir, distance) in path2)
        {
            for (int i = 0; i < distance; i++, counter++)
            {
                current += _directions[dir];

                if (data.TryGetValue(current, out var index))
                    result.Add(metric(current, index, counter));
            }
        }

        return [..result];
    }

    private static int DC2D(char dc) => dc switch
    {
        'U' => 0,
        'R' => 1,
        'D' => 2,
        'L' => 3,

        _ => throw new NotImplementedException()
    };
}
