namespace Puzzles.Runner._2024;

[Puzzle("Monkey Market", 23, 2024)]

public class Day23(ILinesInputReader input) : IPuzzleSolver
{
    private readonly Dictionary<string, List<string>> _connections = [];

    public void Init()
    {
        foreach (var (f, t) in input.GetTokens("-", s => s).Select(t => (t[0], t[1])))
        {
            _connections.GetOrAdd(f, []).Add(t);
            _connections.GetOrAdd(t, []).Add(f);
        }
    }

    public string SolvePart1()
    {
        HashSet<string> paths = [];
        foreach (var key in _connections.Keys.Where(k => k.StartsWith('t')))
            paths.UnionWith(Set(key));

        return paths.Count.ToString();
    }    

    public HashSet<string> Set(string v)
    {        
        HashSet<string> list = new HashSet<string>();

        foreach (var lvl1 in _connections[v].Where(c => c != v))
        {
            foreach (var lvl2 in _connections[lvl1].Where(c => c != lvl1  && c != v))
            {
                foreach (var lvl3 in _connections[lvl2].Where(c => c != lvl2 && c != lvl1))
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
}
