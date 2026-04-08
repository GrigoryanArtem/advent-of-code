namespace Puzzles.Runner._2018;

using System.Text.RegularExpressions;
using Mat = Mat2<int>;

[Puzzle("No Matter How You Slice It", 3, 2018)]
public partial class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private struct Box
    {
        public Box() { }

        public Box(int id, int x, int y, int width, int height)
        {
            Id = id;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int Id { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    private Box[] _boxes = [];

    public void Init()
        => _boxes = [.. input.Lines.Select(Line2Box)];    

    public string SolvePart1()
        => BuildMat(_boxes).Count(d => d > 1).ToString();
    
    public string SolvePart2()
    {
        var mat = BuildMat(_boxes);

        foreach (var box in _boxes)
        {
            var alone = true;

            for (int x = box.X; alone && x < box.Width + box.X; x++)
                for (int y = box.Y; alone && y < box.Height + box.Y; y++)
                    alone &= mat[x, y] == 1;

            if (alone)
                return box.Id.ToString();
        }

        return AOC.NO_ANSWER;
    }

    private static Mat BuildMat(Box[] boxes)
    {
        var mx = boxes.Max(b => b.X + b.Width);
        var my = boxes.Max(b => b.Y + b.Height);

        var mat = Mat.Empty(mx + 1, my + 1);

        foreach (var box in boxes)
            for (int x = box.X; x < box.Width + box.X; x++)
                for (int y = box.Y; y < box.Height + box.Y; y++)
                    mat[x, y]++;

        return mat;
    }

    private static Box Line2Box(string line)
    {
        var match = LineRegex().Match(line);

        return new
        (
            id: Convert.ToInt32(match.Groups["id"].Value),
            x: Convert.ToInt32(match.Groups["x"].Value),
            y: Convert.ToInt32(match.Groups["y"].Value),
            width: Convert.ToInt32(match.Groups["sx"].Value),
            height: Convert.ToInt32(match.Groups["sy"].Value)
        );
    }

    [GeneratedRegex(@"#(?<id>\d+) @ (?<x>\d+),(?<y>\d+): (?<sx>\d+)x(?<sy>\d+)")]
    private static partial Regex LineRegex();
}
