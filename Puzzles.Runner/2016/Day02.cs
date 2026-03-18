namespace Puzzles.Runner._2016;

using Mat = Mat2<char>;

[Puzzle("Bathroom Security", 2, 2016)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private readonly Mat MAP_1 = Mat.WithBorders(
    [
        '1', '2', '3', 
        '4', '5', '6',
        '7', '8', '9'
    ], 3, BORDER);

    private readonly Mat MAP_2 = Mat.WithBorders(
    [
        '#', '#', '1', '#', '#',
        '#', '2', '3', '4', '#',
        '5', '6', '7', '8', '9',
        '#', 'A', 'B', 'C', '#',
        '#', '#', 'D', '#', '#',
    ], 5, BORDER);

    public string SolvePart1()
        => Simulate(MAP_1, new(1, 1));

    public string SolvePart2()
        => Simulate(MAP_2, new(2, 2));

    private string Simulate(Mat map, Vec2 pos)
    {        
        var code = new char[input.Lines.Length];
        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            pos = Simulate(map, pos, line);
            code[idx] = map[pos.X, pos.Y];
        }

        return new string(code);
    }

    private static Vec2 Simulate(Mat map, Vec2 pos, string instructions)
    {        
        foreach(var ch in instructions)
        {
            var next = pos + C2D(ch);
            if (map[next.X, next.Y] != BORDER)
                pos= next;
        }

        return pos;
    }

    private static Vec2 C2D(char ch) => ch switch
    {
        'L' => new(-1, 0),
        'R' => new(1, 0),
        'U' => new(0, -1),
        'D' => new(0, 1),
        _ => default
    };
}
