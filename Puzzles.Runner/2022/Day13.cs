namespace Puzzles.Runner._2022;

[Puzzle("Distress Signal", 13, 2022)]
public partial class Day13(ILinesInputReader input) : IPuzzleSolver
{
    private class Node
    {
        public Node[] Nodes { get; set; } = [];
        public int? Value { get; set; }

        public static Node Parse(string line)
        {
            Parse(line, 1, out var node);
            return node;
        }

        public static Node Create(int value)
            => new() {  Value = value };

        public static Node Create(Node[] nodes)
            => new() { Nodes = nodes };

        private static int Parse(string line, int start, out Node node)
        {            
            List<Node> vals = [];

            var endIndex = 0;
            for (int idx = start; idx < line.Length; idx++)
            {
                var ch = line[idx];

                if (ch == '[')
                {
                    idx = Parse(line, idx + 1, out var val);
                    vals.Add(val);
                    continue;
                }
                else if (ch == ']')
                {
                    endIndex = idx;
                    break;
                }

                if (Char.IsDigit(ch))
                {
                    var numIdx = idx;
                    var next = line[idx + 1];
                    while (next != ',' && next != ']')
                    {
                        idx++;
                        next = line[idx + 1];
                    }

                    vals.Add(Node.Create(Convert.ToInt32(line[numIdx..(idx + 1)])));
                }
            }

            node = Node.Create([.. vals]);
            return endIndex;
        }
    }    

    private Node[] _input = [];

    public void Init()
        => _input = [..input.Lines
            .Where(line => !String.IsNullOrWhiteSpace(line))
            .Select(Node.Parse)];    

    public string SolvePart1()
        => _input
            .Chunk(2)            
            .WithIndex()
            .Where(d => Compare(d.item[0], d.item[1]) > 0)
            .Sum(d => d.index + 1)
            .ToString();

    public string SolvePart2()
    {
        var additional = new Node[] { Node.Parse("[[2]]"), Node.Parse("[[6]]") };
        var comparer = Comparer<Node>.Create(Compare);

        return _input
            .Concat(additional)
            .OrderByDescending(x => x, comparer)
            .WithIndex()
            .Where(d => additional.Contains(d.item))
            .Aggregate(1, (acc, d) => acc * (d.index + 1))
            .ToString();
    }

    private int Compare(Node left, Node right)
    {
        if (left.Value.HasValue && right.Value.HasValue)
            return right.Value.Value.CompareTo(left.Value.Value);

        var ln = left.Value.HasValue ? [left] : left.Nodes;
        var rn = right.Value.HasValue ? [right] : right.Nodes;

        var result = 0;
        var length = Math.Min(rn.Length, ln.Length);
        for (int i = 0; i < length && result == 0; i++)
            result = Compare(ln[i], rn[i]);

        return result == 0 ? rn.Length.CompareTo(ln.Length) : result;
    }
}
