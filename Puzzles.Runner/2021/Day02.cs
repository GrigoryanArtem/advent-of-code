namespace Puzzles.Runner._2021;

[Puzzle("Dive!", 2, 2021)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
    {
        int x = 0;
        int y = 0;

        foreach (var line in input.Lines)
        {
            var tokens = line.Split(' ', 2);
            var d = Convert.ToInt32(tokens[1]);

            switch (tokens[0])
            {
                case "forward":
                    x += d;
                    break;
                case "down":
                    y += d;
                    break;
                case "up":
                    y -= d;
                    break;
            }
        }

        return (x * y).ToString();
    }

    public string SolvePart2()
    {
        long x = 0;
        long y = 0;
        long aim = 0;

        foreach (var line in input.Lines)
        {
            var tokens = line.Split(' ', 2);
            var d = Convert.ToInt32(tokens[1]);

            switch (tokens[0])
            {
                case "forward":
                    x += d;
                    y += d * aim;
                    break;
                case "down":
                    aim += d;
                    break;
                case "up":
                    aim -= d;
                    break;
            }
        }

        return (x * y).ToString();
    }
}
