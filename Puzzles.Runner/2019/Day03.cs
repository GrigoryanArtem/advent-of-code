namespace Puzzles.Runner._2019;

using System.Security.AccessControl;
using Instruction = (int dir, int distance);

[Puzzle("Crossed Wires", 3, 2019)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private Vec2[] _directions = 
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
    {
        var p1 = PathToPoints(_paths[0]);
        var p2 = PathToPoints(_paths[1]);

        p1.IntersectWith(p2);

        return p1.Select(p => AOC.ManhattanDistance(Vec2.Zero, p)).Min().ToString();
    }

    private HashSet<Vec2> PathToPoints(Instruction[] path)
    {
        var result = new HashSet<Vec2> ();
        var current = new Vec2(0, 0);

        foreach (var (dir, distance) in path)
        {
            for (int i = 0; i < distance; i++) 
            {
                current += _directions[dir];
                result.Add(current);
            }
        }

        return result;
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
