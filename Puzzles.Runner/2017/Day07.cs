using System.Text.RegularExpressions;

namespace Puzzles.Runner._2017;

[Puzzle("Recursive Circus", 7, 2017)]
public partial class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const int ERROR = -1;
    private Dictionary<string, int> _ids = [];

    private int[] _weights = [];
    private int[][] _nodes = [];    

    public void Init()
    {       
        _ids.Clear();

        var weights = new List<int>();
        var nodes = new List<List<int>>();

        int AddNode(string name, int weight = 0)
        {
            if(!_ids.TryGetValue(name, out var id))
            {
                id = nodes.Count;

                _ids.Add(name, id);
                nodes.Add([]);
                weights.Add(0);
            }

            if(weight > 0)
                weights[id] = weight;

            return id;
        }
        
        foreach (var line in input.Lines)
        {
            var match = InputRegex().Match(line);

            var nodeId = AddNode(
                match.Groups["name"].Value, 
                Int32.Parse(match.Groups["weight"].Value));

            var connections = match.Groups["connections"].Value.Split(", ", StringSplitOptions.RemoveEmptyEntries);
            foreach (var connection in connections)
                nodes[nodeId].Add(AddNode(connection));            
        }

        _weights = [.. weights];
        _nodes = [.. nodes.Select(x => x.ToArray())];
    }

    public string SolvePart1()
    {
        var bottom = FindBottom();
        return _ids.First(kv => kv.Value == bottom).Key;
    }

    public string SolvePart2()
    {
        var bottom = FindBottom();
        var ww = -1;

        FindWrongWeight(bottom, ref ww);
        return ww.ToString();
    }

    private int FindWrongWeight(int node, ref int wrongWeight)
    {
        if (wrongWeight > 0)
            return 0;

        var sum = 0;
        var buffer = _nodes[node].CreateBuffer<int>();

        foreach (var (connection, idx) in _nodes[node].WithIndex())
        {
            buffer[idx] = FindWrongWeight(connection, ref wrongWeight);

            if (wrongWeight > 0)
                return 0;

            sum += buffer[idx];
        }

        if (buffer.Length > 2)
        {
            var a = buffer[0];
            var b = buffer[1];
            var c = buffer[2];

            var expected = a == b || a == c ? a : b;

            for (int i = 0; i < buffer.Length; i++)
            {
                if (buffer[i] == expected)
                    continue;

                var diff = expected - buffer[i];
                wrongWeight = _weights[_nodes[node][i]] + diff;

                break;
            }
        }

        return _weights[node] + sum;
    }

    private int FindBottom()
    {
        Span<bool> hasParent = stackalloc bool[_nodes.Length];
        hasParent.Clear();

        foreach (var children in _nodes)
            foreach (var child in children)
                hasParent[child] = true;

        for (int id = 0; id < _nodes.Length; id++)
            if (!hasParent[id])
                return id;

        return ERROR;
    }

    [GeneratedRegex(@"(?<name>\w+)\s+\((?<weight>\d+)\)(\s+->\s+(?<connections>(\w+(,\s+)*)+))?")]
    private static partial Regex InputRegex();
}
