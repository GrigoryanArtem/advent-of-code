namespace Puzzles.Runner._2015;

[Puzzle("I Was Told There Would Be No Math", 2, 2015)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => input.Lines.Sum(GetPaperSize).ToString();

    public string SolvePart2()
        => input.Lines.Sum(GetRibbonSize).ToString();

    private int GetPaperSize(string line)
    {
        Parse(line, out var l, out var w, out var h);

        var a = l * w;
        var b = w * h;
        var c = h * l;

        return 2 * (a + b + c) + AOC.Min3(a, b, c);
    }

    private int GetRibbonSize(string line)
    {
        Parse(line, out var l, out var w, out var h);
        return 2 * (l + w + h - AOC.Max3(l, w, h)) + (l * w * h);
    }

    private static void Parse(ReadOnlySpan<char> line, out int l, out int w, out int h)
    {
        var idx = 0;

        l = 0;
        while (line[idx] != 'x')
            l = l * 10 + (line[idx++] - '0');        

        w = 0;
        while (line[++idx] != 'x')
            w = w * 10 + (line[idx] - '0');        

        h = 0;
        while(++idx < line.Length)
            h = h * 10 + (line[idx] - '0');
    }
}
