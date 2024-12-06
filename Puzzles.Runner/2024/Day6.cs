namespace Puzzles.Runner._2024;

[Puzzle("Guard Gallivant", 6, 2024)]
public class Day6(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const int START_DIRECTION = 0;
    private const int HAS_LOOP = -1;

    #endregion

    private record DirectedPoint2D(Point2D Point, int Direction);
    private record Point2D(int X, int Y)
    {
        public static Point2D operator +(Point2D a, Point2D b)
            => new(a.X + b.X, a.Y + b.Y);
    }

    private readonly Point2D[] _directions =
    [
        new(0,  -1),
        new(1,  0),
        new(0,  1),
        new(-1, 0),
    ];

    private Point2D _size = new (0, 0);
    private Point2D _location = new(0, 0);

    private readonly HashSet<Point2D> _obstructions = [];


    public void Init()
    {
        _obstructions.Clear();
        _size = new(input.Lines.Length, input.Lines.First().Length);

        foreach (var (line, line_idx) in input.Lines.Select((ob, i) => (ob, i)))
        {
            foreach(var (item, idx) in line.Select((ch, i) => (ch, i)))
            {
                if (item == '#')
                    _obstructions.Add(new(idx, line_idx));

                if (item == '^')
                    _location = new(idx, line_idx);
            }
        }
    }

    public string SolvePart1()
        => StartMove(_location with { }, START_DIRECTION).ToString();

    public string SolvePart2()
    {
        var sum = 0;
        for(int x = 0; x < _size.X; x++)
        {
            for(int y = 0; y < _size.Y; y++)
            {
                var obstruction = new Point2D(x, y);
                if (_obstructions.Contains(obstruction))
                    continue;

                _obstructions.Add(obstruction);

                var count = StartMove(_location, START_DIRECTION);
                if (count == HAS_LOOP)
                    sum++;

                _obstructions.Remove(obstruction);
            }
        }

        return sum.ToString();
    }

    private int StartMove(Point2D location, int direction)
    {
        HashSet<DirectedPoint2D> visited = [];

        while ((location.X >= 0 && location.X < _size.X) &&
            (location.Y >= 0 && location.Y < _size.Y))
        {
            var next = location + _directions[direction];
            var directidLocation = new DirectedPoint2D(location, direction);

            if (visited.Contains(directidLocation))
                return HAS_LOOP;

            visited.Add(directidLocation);

            if (_obstructions.Contains(next))
            {
                Rotate(ref direction);
            }
            else
            {
                location = next;
            }
        }

        return visited.Select(v => v.point).Distinct().Count();
    }

    private void Rotate(ref int direction)
        => direction = (direction + 1) % _directions.Length;
}
