namespace Puzzles.Runner._2020;

using Map = Mat2<char>;

[Puzzle("Toboggan Trajectory", 3, 2020)]
public partial class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private const char TREE = '#';

    private Map _map = Map.Null;

    public void Init()
    {
        _map = new Map
        (
            [..input.Lines.SelectMany(line => line.AsEnumerable())],
            input.Lines[0].Length
        );
    }

    public string SolvePart1()
        => TreeCount(new(3, 1)).ToString();

    public string SolvePart2()
    {
        Vec2[] slopes =
        [
            new(1, 1),
            new(3, 1),
            new(5, 1),
            new(7, 1),
            new(1, 2),
        ];

        return slopes.Mul(TreeCount).ToString();
    }

    private int TreeCount(Vec2 slope)
    {
        var pos = Vec2.Zero;
        int count = 0;

        while (pos.Y < _map.Rows)
        {
            if (_map[pos.X, pos.Y] == TREE)
                count++;

            pos += slope;
            pos.X %= _map.Columns;
        }

        return count;
    }
}
