namespace Puzzles.Runner._2024;

[Puzzle("Print Queue", 5, 2024)]
public partial class Day5(ILinesInputReader input) : IPuzzleSolver
{
    private class Sorter
    {
        private readonly HashSet<int> _black = [];
        private readonly List<int> _order = [];

        private readonly Dictionary<int, List<int>> _rules;

        private Sorter(Dictionary<int, List<int>> rules)
            => _rules = rules;

        public Dictionary<int, int> Indiceis { get; private set; }

        public static Dictionary<int, int> Sort(Dictionary<int, List<int>> rules)
        {
            var sorter = new Sorter(rules);
            return sorter.Sort().Indiceis;
        }

        private void Sort(int num)
        {
            if (_black.Contains(num))
                return;

            _black.Add(num);

            foreach (var to in _rules[num])
                Sort(to);

            _order.Add(num);
        }

        private Sorter Sort()
        {
            _black.Clear();
            _order.Clear();

            foreach (var num in _rules.Keys)
                Sort(num);

            _order.Reverse();
            Indiceis = _order.Select((v, i) => (v, i)).ToDictionary(d => d.v, d => d.i);

            return this;
        }
    }

    private readonly Dictionary<int, List<int>> _rules = [];
    private readonly List<int[]> _printOrders = [];

    private Dictionary<int,int> _order = [];

    public void Init()
    {
        _rules.Clear();
        _printOrders.Clear();

        int idx = 0;
        for(;idx < input.Lines.Length && !String.IsNullOrWhiteSpace(input.Lines[idx]); idx++)
        {
            var tokens = input.Lines[idx].Split('|').Select(t => Convert.ToInt32(t.Trim()));
            var (from, to) = (tokens.First(), tokens.Last());

            _rules.TryAdd(from, []);
            _rules.TryAdd(to, []);

            _rules[from].Add(to);
        }

        idx++;
        for(;idx < input.Lines.Length; idx++)
        {
            var tokens = input.Lines[idx].Split(',').Select(t => Convert.ToInt32(t.Trim())).ToArray();
            _printOrders.Add(tokens);
        }

        _order = Sorter.Sort(_rules);
    }

    public string SolvePart1()
    {
        return _printOrders.Where(IsOrderCorrect)
            .Select(po => po[po.Length / 2])
            .Sum()
            .ToString();
    }

    public string SolvePart2()
    {
        return _printOrders.Where(po => !IsOrderCorrect(po))
            .Select(Sort)
            .Select(po => po[po.Length / 2])
            .Sum()
            .ToString();
    }

    private bool IsOrderCorrect(int[] order)
        => order.Zip(order.Skip(1), (p, c) => !_rules[c].Contains(p)).All(s => s);

    public int[] Sort(int[] order)
    {
        bool changed;
        do
        {
            changed = false;

            for (int i = 1; i < order.Length; i++)
            {
                if (_rules[order[i]].Contains(order[i - 1]))
                {
                    (order[i], order[i - 1]) = (order[i - 1], order[i]);
                    changed = true;
                }
            }
        } while (changed);

        return order;
    }
}
