namespace Puzzles.Runner._2024;

using Set = HashSet<int>;
using Graph = Dictionary<int, HashSet<int>>;

[Puzzle("LAN Party", 23, 2024)]
public class Day23(ILinesInputReader input) : IPuzzleSolver
{
    private const int T = 't' - 'a';
    private readonly Graph _graph = [];

    public void Init()
    {
        foreach (var (v1, v2) in input.GetTokens("-", s => s).Select(t => (V2I(t[0]), V2I(t[1]))))
        {
            _graph.GetOrAdd(v1, []).Add(v2);
            _graph.GetOrAdd(v2, []).Add(v1);
        }
    }

    public string SolvePart1()
    {
        Set paths = [];

        _graph.Keys.Where(k => StartsWith(k, T))
            .ForEach(k => paths.UnionWith(FindSet(k)));

        return paths.Count.ToString();
    }

    public string SolvePart2()
        => String.Join(",", Cliques(_graph).MaxBy(c => c.Count)?.Select(I2V).OrderBy(x => x)!);

    public Set FindSet(int v)
    {        
        Set list = [];

        foreach (var lvl1 in _graph[v])
            foreach (var lvl2 in _graph[lvl1])
                foreach (var lvl3 in _graph[lvl2].Where(c => c == v))
                    list.Add(ToSet(v, lvl1, lvl2));

        return list;
    }

    private static List<Set> Cliques(Graph graph)
    {
        var cliques = new List<Set>();
        BronKerbosch([], new(graph.Keys), [], graph, cliques);

        return cliques;
    }

    // https://en.wikipedia.org/wiki/Bron%E2%80%93Kerbosch_algorithm
    private static void BronKerbosch(Set R, Set P, Set X, Graph graph, List<Set> cliques)
    {
        if (P.Count == 0 && X.Count == 0)
        {            
            cliques.Add(R);
            return;
        }

        int pivot = ChoosePivot(P, X, graph); 
        foreach (var v in P.Except(graph[pivot]))
        {            
            BronKerbosch
            (
                R: new(R) { v }, 
                P: new(P.Intersect(graph[v])), 
                X: new(X.Intersect(graph[v])), 
                graph: graph,
                cliques: cliques
            );

            P.Remove(v);
            X.Add(v);
        }
    }

    private static int ChoosePivot(Set P, Set X, Graph graph)
        => P.Union(X).OrderByDescending(v => graph[v].Count).First();

    private static int ToSet(int v1, int v2, int v3)
    {
        AOC.Sort3(ref v1, ref v2, ref v3);
        return (((v1 << 10) + v2) << 10) + v3;
    }

    private static int V2I(string s)
        => ((s[0] - 'a') << 5) + (s[1] - 'a');

    private static string I2V(int id)
        => new ([(char)((id >> 5) + 'a'), (char)((id & 31) + 'a' )]);

    private static bool StartsWith(int v, int target)
        => (v >> 5) == target;

}
