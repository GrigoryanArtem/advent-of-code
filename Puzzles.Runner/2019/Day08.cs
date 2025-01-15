namespace Puzzles.Runner._2019;

[Puzzle("Space Image Format", 8, 2019)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private record Layer(int[] Data, IReadOnlyDictionary<int, int> count);

    private Layer[] _layers = []; 

    public void Init()
    {
        _layers = input.Lines.First().Select(c => c - '0')
            .Chunk(25 * 6)
            .Select(c => new Layer([.. c], c.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count())))
            .ToArray();
    }

    public string SolvePart1()
    {
        var layer = _layers.MinBy(layer => layer.count.GetValueOrDefault(0));
        return (layer.count.GetValueOrDefault(1) * layer.count.GetValueOrDefault(2)).ToString();
    }
}
