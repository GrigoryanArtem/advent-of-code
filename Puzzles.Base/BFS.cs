using Puzzles.Base.Entities;

namespace Puzzles.Base;

public class BFS<T>(Map2<T> map, HashSet<T> obstructions)
{
    private readonly Queue<int> queue = new(map.Data.Length);
    private readonly HashSet<int> visited = new(map.Data.Length);

    public int[] Full(int start, int[] distances)
    {
        Array.Fill(distances, Int32.MaxValue);

        queue.Clear();
        visited.Clear();

        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.TryDequeue(out var current))
        {
            if (obstructions.Contains(map[current]) || visited.Contains(current))
                continue;

            visited.Add(current);
            Array.ForEach(map.Directions, d =>
            {
                var next = current + d;
                var nextD = distances[current] + 1;

                if (nextD < distances[next])
                {
                    distances[next] = nextD;
                    queue.Enqueue(next);
                }
            });
        }

        return distances;
    }
}
