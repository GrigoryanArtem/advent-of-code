namespace Puzzles.Runner._2024;

[Puzzle("Historian Hysteria", 1, 2024)]
public partial class Day01(ILinesInputReader input) : IPuzzleSolver
{
    private readonly List<int> first = [];
    private readonly List<int> second = [];

    private readonly Dictionary<int, uint> counter = [];

    public void Init()
    {
        var tokens = input.GetTokens(" ", Convert.ToInt32);

        Array.ForEach(tokens, t =>
        {
            var last = t.Last();

            first.Add(t.First());
            second.Add(last);

            counter.TryAdd(last, 0);
            counter[last]++;
        });

        first.Sort();
        second.Sort();
    }

    public string SolvePart1()
        => first.Zip(second, (f, s) => Math.Abs(s - f)).Sum().ToString();

    public string SolvePart2()
        => first.Aggregate(0UL, (acc, v) => acc += (uint)(v * counter.GetValueOrDefault(v, 0U))).ToString();
}
