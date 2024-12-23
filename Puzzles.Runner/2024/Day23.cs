namespace Puzzles.Runner._2024;

using Set = HashSet<string>;
using Graph = Dictionary<string, HashSet<string>>;

[Puzzle("Monkey Market", 23, 2024)]
public class Day23(ILinesInputReader input) : IPuzzleSolver
{
    private readonly Graph _graph = [];

    public void Init()
    {
        foreach (var (v1, v2) in input.GetTokens("-", s => s).Select(t => (t[0], t[1])))
        {
            _graph.GetOrAdd(v1, []).Add(v2);
            _graph.GetOrAdd(v2, []).Add(v1);
        }
    }

    public string SolvePart1()
    {
        Set paths = [];
        foreach (var key in _graph.Keys.Where(k => k.StartsWith('t')))
            paths.UnionWith(FindSet(key));

        return paths.Count.ToString();
    }

    public string SolvePart2()
        => String.Join(",", Cliques(_graph).MaxBy(c => c.Count)?.OrderBy(x => x)!);

    public Set FindSet(string v)
    {        
        Set list = [];

        foreach (var lvl1 in _graph[v].Where(c => c != v))
        {
            foreach (var lvl2 in _graph[lvl1].Where(c => c != lvl1  && c != v))
            {
                foreach (var lvl3 in _graph[lvl2].Where(c => c != lvl2 && c != lvl1))
                {
                    if (lvl3 == v)
                    {
                        var path = new string[] { v, lvl1, lvl2 }.OrderBy(c => c).ToArray();
                        list.Add(String.Join(",", path));
                    }
                }
            }
        }
        
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
        
        foreach (var v in P)
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
}
