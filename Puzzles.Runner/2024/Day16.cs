using Map = Puzzles.Base.Entites.Map2<char>;
using Node = (int location, int direction, int distance);

namespace Puzzles.Runner._2024;

[Puzzle("Reindeer Maze", 16, 2024)]
public class Day16(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private const char START = 'S';
    private const char END = 'E';

    private const int FORWARD = 1;
    private const int ROTATE = 1000;

    private Map? _map;
    private int[] _buffer = [];

    public void Init()
    {
        _map = new(data: [.. input.Lines.SelectMany(line => line)], columns: input.Lines.First().Length);
        _buffer = new int[_map.Data.Length * _map.Directions.Length];
    }

    public string SolvePart1()
    {
        var end = Array.IndexOf(_map!.Data, END);
        DistanceToEnd(_map, Array.IndexOf(_map.Data, START), end, _buffer);
        return GetMinDistance(_map, _buffer, end).ToString();
    }

    public string SolvePart2()
    {
        var start = Array.IndexOf(_map!.Data, START);
        var end = Array.IndexOf(_map.Data, END);

        DistanceToEnd(_map, Array.IndexOf(_map.Data, START), end, _buffer);

        var nodes = new HashSet<int>();
        var lds = GetMinIndices(_map, _buffer, end);
        lds.ForEach(ld => VisitPlaces(_map, _buffer, ld, _map.L2LD(start, Map.RIGHT), nodes));

        nodes.Add(_map.L2LD(start, Map.RIGHT));
        return nodes.Select(ld => _map.LD2L(ld).location)
            .Distinct()
            .Count()
            .ToString();
    }

    private static void VisitPlaces(Map map, int[] distances, int currentLD, int endLD, HashSet<int> visited)
    {
        if (visited.Contains(currentLD) | currentLD == endLD)
            return ;

        visited.Add(currentLD);
        var (location, direction) = map.LD2L(currentLD);        

        var forward = new[] 
        {
            AOC.Mod(direction + 2, map.Directions.Length)
        };

        var rotations = new int[]
        {
            AOC.Mod(direction + 1, map.Directions.Length),
            AOC.Mod(direction + 3, map.Directions.Length),
        };

        rotations.Select(d => map.L2LD(location, d))
            .Concat(forward.Select(d => map.L2LD(map.Next(location, d),
                AOC.Mod(d + 2, map.Directions.Length))))
            .Where(p => distances[currentLD] - distances[p] == FORWARD || distances[currentLD] - distances[p] == ROTATE)
            .ForEach(n => VisitPlaces(map, distances, n, endLD, visited));
    }

    private static void DistanceToEnd(Map map, int start, int end, int[] distances)
    {
        Array.Fill(distances, int.MaxValue);

        var visited = new HashSet<int>();
        var pq = new PriorityQueue<Node, int>();

        pq.Enqueue((start, 1, 0), 0);
        distances[start] = 0;

        while (pq.Count > 0)
        {
            var (loc, dir, distance) = pq.Dequeue();
            var ld = map.L2LD(loc, dir);

            if (visited.Contains(ld) || map[loc] == BORDER)
                continue;

            distances[ld] = distance;
            visited.Add(ld);

            if (loc == end)
                continue;

            pq.Enqueue((map.Next(loc, dir), dir, distance + FORWARD), distance + FORWARD);

            var nd = AOC.Mod(dir + 1, map.Directions.Length);
            var pd = AOC.Mod(dir + 3, map.Directions.Length);

            pq.Enqueue((loc, nd, distance + ROTATE), distance + ROTATE);
            pq.Enqueue((loc, pd, distance + ROTATE), distance + ROTATE);
        }
    }

    private static int GetMinDistance(Map map, int[] distances, int end)
    {
        var endUp = map.L2LD(end, Map.UP);
        return Enumerable.Range(0, map.Directions.Length).Min(ld => distances[endUp + ld]);
    }

    private static int[] GetMinIndices(Map map, int[] distances, int end)
    {
        var endUp = map.L2LD(end, Map.UP);
        var directions = Enumerable.Range(0, map.Directions.Length);
        var min = directions.Min(ld => distances[endUp + ld]);
        return directions.Where(ld => distances[endUp + ld] == min).Select(ld => ld + endUp).ToArray();
    }
}
