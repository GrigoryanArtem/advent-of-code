namespace Puzzles.Runner._2022;

using Map = Mat2<short>;

[Puzzle("Treetop Tree House", 8, 2022)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private Map _map = Map.Null;

    public void Init()
    {
        _map = new Map
        (
            [.. input.Lines.SelectMany(line => line.AsEnumerable().Select(ch => (short)(ch - '0')))],
            input.Lines.First().Length
        );
    }

    public string SolvePart1()
    {
        var map = _map.Copy();
        var set = new HashSet<int>();

        foreach (var (el, idx) in map.WithIndex())
        {
            foreach (var ddx in Enumerable.Range(0, 4))
            {
                if (ToBorder(map, idx, ddx).All(n => n < el))
                    set.Add(idx);
            }
        }

        return (set.Count).ToString();
    }

    public string SolvePart2()
    {
        var map = _map.Copy();
        var array = map.WithIndex()
            .AsParallel()
            .Select(d => Enumerable.Range(0, 4)
                .Aggregate(1, (acc, ddx) => acc *= ToBorder(map, d.index, ddx).IndexOf(n => n >= d.item)))
            .ToArray();

        return array.Max().ToString();
    }

    public static IEnumerable<short> ToBorder(Map map, int idx, int ddx)
    {
        int y = idx / map.Columns;

        while (true)
        {
            int nextIdx = map.Next(idx, ddx);

            if (nextIdx < 0 || nextIdx >= map.Data.Length)
                yield break;

            int nextY = nextIdx / map.Columns;

            if (ddx % 2 != 0 && nextY != y)
                yield break;

            idx = nextIdx;

            yield return map[idx];
        }
    }
}
