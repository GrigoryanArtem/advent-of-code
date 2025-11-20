namespace Puzzles.Runner._2021;

[Puzzle("Syntax Scoring", 10, 2021)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private enum Mode { Errors, Complete };
    
    private Dictionary<char, char> _braces = new()
    {
        ['['] = ']',
        ['<'] = '>',
        ['{'] = '}',
        ['('] = ')'

    };

    private static Dictionary<char, ulong> _p1scores = new()
    {
        [')'] = 3,
        [']'] = 57,
        ['}'] = 1197,
        ['>'] = 25137,
    };

    private static Dictionary<char, ulong> _p2scores = new()
    {
        [')'] = 1,
        [']'] = 2,
        ['}'] = 3,
        ['>'] = 4
    };

    public string SolvePart1()
        => input.Lines.UInt64Sum(line => TryComplete(line, Mode.Errors)).ToString();
    

    public string SolvePart2()
    {
        var scores = input.Lines.Select(line => TryComplete(line, Mode.Complete))
            .Where(v => v > 0)            
            .ToList();

        scores.Sort();

        return scores[scores.Count / 2].ToString();
    }

    private ulong TryComplete(string line, Mode mode)
    {
        Span<char> _stack = stackalloc char[2048];
        int ptr = 0;

        foreach (var ch in line)
        {
            if (_braces.TryGetValue(ch, out var opposite))
            {
                _stack[ptr++] = opposite;
            }
            else
            {
                var pop = _stack[--ptr];
                if (pop != ch)
                {
                    return mode == Mode.Errors ? C2S1(ch) : 0;
                }

            }
        }
        
        return mode == Mode.Complete && ptr > 0 ? CompleteToScore(_stack, ptr) : 0;
    }


    private static ulong CompleteToScore(Span<char> stack, int size)
    {
        var sum = 0UL;

        for (int i = size - 1; i >= 0; i--)
            sum = sum * 5 + C2S2(stack[i]);

        return sum;
    }

    private static ulong C2S2(char ch)
        => _p2scores[ch];

    private static ulong C2S1(char ch)
        => _p1scores[ch];
}
