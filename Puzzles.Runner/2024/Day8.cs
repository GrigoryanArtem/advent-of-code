namespace Puzzles.Runner._2024;


[Puzzle("Bridge Repair", 8, 2024)]
public class Day8(ILinesInputReader input) : IPuzzleSolver
{
    private record Point2D(int X, int Y)
    {
        public static Point2D operator +(Point2D a, Point2D b)
            => new(a.X + b.X, a.Y + b.Y);

        public static Point2D operator -(Point2D a, Point2D b)
            => new(a.X - b.X, a.Y - b.Y);

        public void Deconstruct(out int x, out int y)
        {
            x = X;
            y = Y;
        }
    }

    private const char EMPTY_CELL = '.';

    private Dictionary<char, List<Point2D>> _antennas = [];

    private int SizeY { get; set; }
    private int SizeX { get; set; }

    public void Init()
    {
        _antennas.Clear();

        SizeY = input.Lines.Length;
        SizeX = input.Lines[0].Length;

        foreach (var (line, y) in input.Lines.WithIndex())
        {
            foreach(var (ch, x) in line.WithIndex().Where(c => c.Item1 != EMPTY_CELL))
            {
                _antennas.TryAdd(ch, []);
                _antennas[ch].Add(new(x, y));
            }
        }
    }

    public string SolvePart1()
    {
        HashSet<Point2D> antinodes = [];

        foreach(var (antenna, positions) in _antennas)
        {
            for(int i = 0; i < positions.Count; i++)
            {
                for (int k = i + 1; k < positions.Count; k++) 
                { 
                    var d = positions[i] - positions[k];

                    var a1 = positions[i] + d;
                    var a2 = positions[k] - d;

                    if (a1.Y >= 0 && a1.X >=0
                        && a1.Y < SizeY && a1.X < SizeX)
                        antinodes.Add(a1);

                    if ( a2.Y >= 0 && a2.X >= 0
                        && a2.Y < SizeY && a2.X < SizeX)
                        antinodes.Add(a2);
                }
            }            
        }

        return antinodes.Count.ToString();
    }
}
