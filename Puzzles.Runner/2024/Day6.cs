namespace Puzzles.Runner._2024;

[Puzzle("Guard Gallivant", 6, 2024)]
public class Day6(ILinesInputReader input) : IPuzzleSolver
{
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

    private Point2D _size;
    private Point2D _location;
    private int _direction;

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
    {        
        HashSet<Point2D> _visited = [];

        while((_location.X >= 0 && _location.X < _size.X) &&
            (_location.Y >= 0 && _location.Y < _size.Y))
        {
            var next = _location + _directions[_direction];

            if (_obstructions.Contains(next))
            {
                Rotate();
            }
            else
            {                
                _location = next;
                _visited.Add(next);
            }
        }

        return (_visited.Count - 1).ToString();
    }

    private void Rotate()
        => _direction = (_direction + 1) % _directions.Length;

}
