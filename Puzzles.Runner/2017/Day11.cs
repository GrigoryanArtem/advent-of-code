namespace Puzzles.Runner._2017;

[Puzzle("Hex Ed", 11, 2017)]
public class Day11(IFullInputReader input) : IPuzzleSolver
{
    private static class Directions
    {
        public static readonly HexAx N = new(0, -1);
        public static readonly HexAx S = new(0, +1);
        public static readonly HexAx NE = new(+1, -1);
        public static readonly HexAx SW = new(-1, +1);
        public static readonly HexAx SE = new(+1, 0);
        public static readonly HexAx NW = new(-1, 0);
    }

    public string SolvePart1()
        => Simulate(input.Text.AsSpan()).final.ToString();    

    public string SolvePart2()
        => Simulate(input.Text.AsSpan()).max.ToString();

    private static (int final, int max) Simulate(ReadOnlySpan<char> input)
    {
        var pos = HexAx.Zero;
        var max = 0;

        for (int i = 0; i < input.Length;)
        {
            var val = input[i++];

            if (val is ',' or '\r' or '\n')
                continue;

            var next = i < input.Length ? input[i] : (char)0;

            switch (val)
            {
                case 'n':
                    if (next == 'e')
                    {
                        pos.Add(Directions.NE);
                        i++;
                    }
                    else if (next == 'w')
                    {
                        pos.Add(Directions.NW);
                        i++;
                    }
                    else
                        pos.Add(Directions.N);
                    break;

                case 's':
                    if (next == 'e')
                    {
                        pos.Add(Directions.SE);
                        i++;
                    }
                    else if (next == 'w')
                    {
                        pos.Add(Directions.SW);
                        i++;
                    }
                    else
                        pos.Add(Directions.S);
                    break;
            }

            max = Math.Max(max, pos.HexLength());
        }

        return (pos.HexLength(), max);
    }
}
