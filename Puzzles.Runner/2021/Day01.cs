namespace Puzzles.Runner._2021;

[Puzzle("Sonar Sweep", 1, 2021)]
public class Day01(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => Calcaulte(1).ToString();

    public string SolvePart2()
        => Calcaulte(3).ToString();

    private int Calcaulte(int windowSize)
    {
        var nums = input.Lines.Select(x => Convert.ToInt32(x)).ToArray();

        int count = 0;
        var prev = nums.Take(windowSize).Sum();
        for (int i = windowSize; i < nums.Length; i++)
        {
            var curr = prev + nums[i] - nums[i - windowSize];            

            if(curr > prev)
                count++;

            prev = curr;
        }

        return count;
    }
}
