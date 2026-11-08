namespace Puzzles.Runner._2017;

[Puzzle("Stream Processing", 9, 2017)]
public class Day09(IFullInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => Process(input.Text.AsSpan()).score.ToString();
    
    public string SolvePart2()
        => Process(input.Text.AsSpan()).garbage.ToString();

    private static (int score, int garbage) Process(ReadOnlySpan<char> input)
    {
        var score = 0;
        var garbage = 0;

        var depth = 0;
        var garbageMode = false;

        for (var i = 0; i < input.Length; i++)
        {
            var ch = input[i];

            if (garbageMode)
            {
                switch (ch)
                {
                    case '!':
                        i++;
                        break;

                    case '>':
                        garbageMode = false;
                        break;

                    default:
                        garbage++;
                        break;
                }
            }
            else
            {
                switch (ch)
                {
                    case '{':
                        score += ++depth;
                        break;

                    case '}':
                        depth--;
                        break;

                    case '<':
                        garbageMode = true;
                        break;
                }
            }
        }

        return (score, garbage);
    }
}
