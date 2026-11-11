namespace Puzzles.Runner._2017;

[Puzzle("Digital Plumber", 12, 2017)]
public class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private class UnionFind
    {
        private readonly int[] _parent;
        private readonly int[] _size;

        public UnionFind(int size)
        {
            _parent = new int[size];
            _size = new int[size];

            for(int i = 0; i < size; i++)
            {
                _parent[i] = i;
                _size[i] = 1;
            }

            Groups = size;
        }

        public int Groups { get; private set; }

        public void Union(int a, int b)
        {
            a = Find(a);
            b = Find(b);

            if (a == b)
                return;

            if (b < a)
                (a, b) = (b, a);

            _parent[b] = a;
            _size[a] += _size[b];

            Groups--;
        }

        public int Find(int v)
        {
            while (_parent[v] != v)
            {
                _parent[v] = _parent[_parent[v]];
                v = _parent[v];
            }

            return v;
        }

        public int SizeOf(int v)
            => _size[Find(v)];
    }

    private record Door(int Id, int[] Connections);

    private Door[] _doors = [];

    public void Init()
    {
        var n = input.Lines.Length;
        _doors = new Door[n];

        for(int i = 0; i < n; i++)
        {
            var tokens = input.Lines[i].Split("<->", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToArray();

            _doors[i] = new
            (
                Int32.Parse(tokens[0]),
                [.. Parse.StringToNumbers(tokens[1])]
            );
        }
    }

    public string SolvePart1()
    {
        var uf = BuildUnionFind();
        return uf.SizeOf(0).ToString();
    }

    public string SolvePart2()
    {
        var uf = BuildUnionFind();
        return uf.Groups.ToString();
    }

    private UnionFind BuildUnionFind()
    {
        var uf = new UnionFind(_doors.Length);

        foreach (var door in _doors)
            foreach (var connection in door.Connections)
                uf.Union(door.Id, connection);

        return uf;
    }
}
