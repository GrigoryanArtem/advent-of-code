namespace Puzzles.Runner._2022;

[Puzzle("Rope Bridge", 9, 2022)]
public class Day09(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => SimulateRope(2).ToString();

    public string SolvePart2()
        => SimulateRope(10).ToString();

    private int SimulateRope(int length)
    {
        var rope = Enumerable.Range(0, length).Select(_ => new Vec2()).ToArray();

        HashSet<Vec2> set = [rope.Last()];

        foreach (var line in input.Lines)
        {
            var d = Ch2D(line[0]);
            var count = Convert.ToInt32(line[2..]);

            for (var i = 0; i < count; i++)
            {
                rope[0] += d;

                for (int k = 1; k < rope.Length; k++)
                {
                    var prev = rope[k - 1];
                    var cur = rope[k];

                    if (AOC.ChebyshevDistance(cur, prev) > 1)
                    {
                        var dir = (prev - cur).Clamp(1);
                        rope[k] += dir;
                    }
                }

                set.Add(rope.Last());
            }
        }

        return set.Count;
    }


    public static Vec2 Ch2D(char d) => d switch
    {
        'R' => new(1, 0),
        'L' => new(-1, 0),

        'U' => new(0, 1),
        'D' => new(0, -1),

        _ => throw new NotImplementedException(),
    };
}
