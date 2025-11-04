namespace Puzzles.Runner._2022;

[Puzzle("Calorie Counting", 1, 2022)]
public class Day01(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => SumOfCalories().Max().ToString();    

    public string SolvePart2()
        => TopNSum(3).ToString();

    public long TopNSum(int n)
    {
        var pq = new PriorityQueue<long, long>(n + 1);

        foreach(var sum in SumOfCalories())
        {
            pq.Enqueue(sum, sum);

            if (pq.Count > n)
                pq.Dequeue();
        }

        return pq.UnorderedItems.Sum(x => x.Element);
    }

    public IEnumerable<long> SumOfCalories()
    {
        var current = 0L;

        foreach (var line in input.Lines)
        {
            if (String.IsNullOrEmpty(line))
            {
                yield return current;

                current = 0;
                continue;
            }

            current += Convert.ToInt64(line);
        }

        yield return current;
    }
}
