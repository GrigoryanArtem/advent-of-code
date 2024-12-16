using Map = Puzzles.Base.Entites.Map2<char>;
using Node = (int location, int direction, int distance);


namespace Puzzles.Runner._2024;

[Puzzle("Reindeer Maze", 16, 2024)]
public class Day16(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private const char START = 'S';
    private const char END = 'E';
    private const int NO_PATH = -1;

    private Map _map;

    public void Init()
        => _map = new(data: [..input.Lines.SelectMany(line => line)], columns: input.Lines.First().Length);
    

    public string SolvePart1()
        => DistanceToEnd(_map, Array.IndexOf(_map.Data, START)).ToString();
    

    private int DistanceToEnd(Map map, int start)
    {
        var visited = new HashSet<int>();
        var pq = new PriorityQueue<Node, int>();

        pq.Enqueue((start, 1, 0), 0);

        while(pq.Count > 0)
        {
            var (loc, dir, distance) = pq.Dequeue();

            if (visited.Contains(loc) || map[loc] == BORDER)
                continue;

            if (map[loc] == END)
                return distance;

            visited.Add(loc);

            pq.Enqueue((map.Next(loc, dir), dir, distance + 1), distance + 1);

            var nd = (dir + 1) % map.Directions.Length;
            var pd = (dir + 3) % map.Directions.Length;

            pq.Enqueue((map.Next(loc, nd), nd, distance + 1001), distance + 1001);
            pq.Enqueue((map.Next(loc, pd), pd, distance + 1001), distance + 1001);
        }

        return NO_PATH;
    }
}
