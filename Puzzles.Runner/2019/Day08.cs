using System.Text;

namespace Puzzles.Runner._2019;

[Puzzle("Space Image Format", 8, 2019)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private record Layer(int[] Data, IReadOnlyDictionary<int, int> Count);

    private const int WIDTH = 25;
    private const int HEIGHT = 6;

    private Layer[] _layers = []; 

    public void Init()
        => _layers = input.Lines.First().Select(c => c - '0')
            .Chunk(WIDTH * HEIGHT)
            .Select(c => new Layer([.. c], c.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count())))
            .ToArray();

    public string SolvePart1()
    {
        var layer = _layers.MinBy(layer => layer.Count.GetValueOrDefault(0))!;
        return (layer.Count.GetValueOrDefault(1) * layer.Count.GetValueOrDefault(2)).ToString();
    }

    public string SolvePart2()
        => DrawImage(MergeLayers(_layers));    

    #region Private methods

    private static int[] MergeLayers(Layer[] layers)
       => Enumerable.Range(0, WIDTH * HEIGHT)
       .Select(i => layers.FirstOrDefault(layer => layer.Data[i] != 2)?.Data[i] ?? 2)
       .ToArray();

    private static string DrawImage(int[] image)
    {
        StringBuilder sb = new();

        for (int i = 0; i < image.Length; i++)
        {
            if(i % WIDTH == 0)
                sb.AppendLine();

            sb.Append(C2S(image[i]));
        }

        return sb.ToString();
    }

    private static char C2S(int c) => c switch
    {
        0 => ' ',
        1 => '#',
        _ => '.'
    };

    #endregion
}
