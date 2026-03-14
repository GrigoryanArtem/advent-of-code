namespace Puzzles.Runner._2015;

[Puzzle("Not Quite Lisp", 1, 2015)]
public class Day01(IFullInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => (2 * input.Text.Count(ch => (~ch & 1) > 0) - input.Text.Length).ToString();

    public string SolvePart2()
    {
        var floor = 0;
        foreach (var (ch, idx) in input.Text.WithIndex())
        {
            floor += ((~ch & 1) << 1) - 1;

            if (floor == -1)
                return (idx + 1).ToString();
        }

        return AOC.NO_ANSWER;
    }
}
