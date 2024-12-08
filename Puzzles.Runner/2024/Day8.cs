using Puzzles.Base.Entites;

namespace Puzzles.Runner._2024;

[Puzzle("Bridge Repair", 8, 2024)]
public class Day8(ILinesInputReader input) : IPuzzleSolver
{  
    private const char EMPTY_CELL = '.';
    private readonly Dictionary<char, List<Point2>> _antennas = [];

    private int SizeY { get; set; }
    private int SizeX { get; set; }

    public void Init()
    {
        _antennas.Clear();

        SizeY = input.Lines.Length;
        SizeX = input.Lines[0].Length;

        input.Lines.WithIndex()
            .ForEach(line => line.item.WithIndex()
                .Where(c => c.item != EMPTY_CELL)
                .ForEach(ch => 
                {
                    _antennas.TryAdd(ch.item, []);
                    _antennas[ch.item].Add(new(ch.index, line.index));
                }));
    }

    public string SolvePart1()
        => CountAntinodes(infinite: false).ToString();

    public string SolvePart2()
        => CountAntinodes().ToString();

    private int CountAntinodes(bool infinite = true )
    {
        HashSet<Point2> antinodes = [];

        foreach (var positions in _antennas.Values)
        {
            for (int i = 0; i < positions.Count; i++)
            {
                for (int k = i + 1; k < positions.Count; k++)
                {
                    var d = positions[i] - positions[k];                   
                    for (var (added, t) = (true, infinite ? 0 : 1); added; t++)
                    {
                        var a1 = AddAntinode(antinodes, positions[i] + (d * t));
                        var a2 = AddAntinode(antinodes, positions[k] - (d * t));
                        added = infinite && (a1 | a2);
                    }
                }
            }
        }

        return antinodes.Count;
    }

    private bool AddAntinode(HashSet<Point2> hashSet, Point2 point)
    {
        var canAdd = point.Y >= 0 && point.X >= 0 && point.Y < SizeY && point.X < SizeX;        
        var _ = canAdd && hashSet.Add(point);

        return canAdd;
    }
}
