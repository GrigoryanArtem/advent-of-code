namespace Puzzles.Runner._2024;

[Puzzle("Print Queue", 5, 2024)]
public partial class Day5(ILinesInputReader input) : IPuzzleSolver
{   
    private readonly Dictionary<int, List<int>> _rules = [];
    private readonly List<int[]> _print = [];

    private Comparer<int>? _comparer;

    public void Init()
    {
        _rules.Clear();
        _print.Clear();

        int idx = 0;
        for(;idx < input.Lines.Length && !String.IsNullOrWhiteSpace(input.Lines[idx]); idx++)
        {
            var tokens = input.Lines[idx].Split('|').Select(t => Convert.ToInt32(t.Trim()));
            var (from, to) = (tokens.First(), tokens.Last());

            _rules.TryAdd(from, []);
            _rules.TryAdd(to, []);

            _rules[from].Add(to);
        }
        
        for(idx += 1;idx < input.Lines.Length; idx++)                    
            _print.Add(input.Lines[idx].Split(',').Select(t => Convert.ToInt32(t.Trim())).ToArray());        

        _comparer = Comparer<int>.Create((p, c) => _rules[c].Contains(p) ? 1 : -1);
    }

    public string SolvePart1()
        => _print.Where(IsOrderCorrect)
            .Sum(po => po[po.Length / 2])
            .ToString();

    public string SolvePart2()
        =>  _print.Where(po => !IsOrderCorrect(po))
            .Select(po => po.OrderBy(p => p, _comparer).ToArray())            
            .Sum(po => po[po.Length / 2])
            .ToString();

    private bool IsOrderCorrect(int[] order)
        => order.Zip(order.Skip(1), (p, c) => !_rules[c].Contains(p)).All(s => s);
}
