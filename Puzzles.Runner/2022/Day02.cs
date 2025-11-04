namespace Puzzles.Runner._2022;

[Puzzle("Rock Paper Scissors", 2, 2022)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{    
    public string SolvePart1() => Calculate(Score);
    public string SolvePart2() => Calculate(PrdictScore);

    private string Calculate(Func<int, int, int> func)
        => input.GetTokens(" ", s => s.First())
            .Select(t => (op: t[0] - 'A', me: t[1] - 'X'))
            .Sum(x => func(x.op, x.me))
            .ToString();

    private static int Score(int op, int me)
        => (me + 1) + AOC.Mod(me - op + 1, 3) * 3;
    private static int PrdictScore(int op, int me)
        => (me * 3) + AOC.Mod(op + me - 1, 3) + 1;    
}
