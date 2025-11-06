namespace Puzzles.Runner._2022;

[Puzzle("Rope Bridge", 9, 2022)]
public class Day09(ILinesInputReader input) : IPuzzleSolver
{
    private readonly static Dictionary<char, Vec2> DIRECTIONS = new()
    {
        ['R'] = new(1, 0),
        ['L'] = new(-1, 0),
        ['U'] = new(0, 1),
        ['D'] = new(0, -1),
    };

    public string SolvePart1()
        => SimulateRope(2).ToString();

    public string SolvePart2()
        => SimulateRope(10).ToString();

    private int SimulateRope(int length)
    {
        var rope = new Vec2[length];

        ref var head = ref rope[0];
        ref var tail = ref rope[length - 1];

        HashSet<Vec2> set = [tail];

        foreach (var line in input.Lines)
        {
            var count = Convert.ToInt32(line[2..]);
            for (var i = 0; i < count; i++)
            {
                head.Move(DIRECTIONS[line[0]]);

                for (int k = 1; k < rope.Length; k++)
                {
                    ref var prev = ref rope[k - 1];
                    ref var cur = ref rope[k];

                    if (AOC.ChebyshevDistance(cur, prev) > 1)
                    {
                        var dir = (prev - cur).Clamp(1);
                        rope[k].Move(dir);
                    }
                }

                set.Add(tail);
            }
        }

        return set.Count;
    }
}
