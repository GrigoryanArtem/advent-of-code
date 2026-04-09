namespace Puzzles.Runner._2018;

[Puzzle("Alchemical Reduction", 5, 2018)]
public partial class Day05(IFullInputReader input) : IPuzzleSolver
{
    private const int UP_LOW_SHIFT = 0x20;

    public string SolvePart1()
    {                
        Span<char> stack = stackalloc char[input.Text.Length];

        return BuildPolymer(input.Text, stack).ToString();
    }

    public string SolvePart2()
    {
        var span = input.Text.AsSpan();
        Span<char> stack = stackalloc char[input.Text.Length];

        var min = stack.Length;
        for (char c = 'a'; c <= 'z'; c++)
            min = Math.Min(min, BuildPolymer(span, stack, c));

        return min.ToString();
    }

    private static int BuildPolymer(ReadOnlySpan<char> span, Span<char> buffer, char skipLo = Char.MinValue)
    {        
        var sidx = 0;
        for (int i = 0; i < span.Length; i++)
        {
            if ((span[i] | UP_LOW_SHIFT) == skipLo)
                continue;

            if ((buffer[sidx] ^ span[i]) == UP_LOW_SHIFT)
            {
                sidx--;
            }
            else
            {
                buffer[++sidx] = span[i];
            }
        }

        return sidx;
    } 
}
