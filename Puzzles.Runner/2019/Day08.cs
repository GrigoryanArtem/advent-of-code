using System.Text;

namespace Puzzles.Runner._2019;

[Puzzle("Space Image Format", 8, 2019)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private record Layer(int[] Data, IReadOnlyDictionary<int, int> count);

    private const int WIDTH = 25;
    private const int HEIGHT = 6;

    private Layer[] _layers = []; 

    public void Init()
    {
        _layers = input.Lines.First().Select(c => c - '0')
            .Chunk(WIDTH * HEIGHT)
            .Select(c => new Layer([.. c], c.GroupBy(x => x)
                .ToDictionary(x => x.Key, x => x.Count())))
            .ToArray();
    }

    public string SolvePart1()
    {
        var layer = _layers.MinBy(layer => layer.count.GetValueOrDefault(0));
        return (layer.count.GetValueOrDefault(1) * layer.count.GetValueOrDefault(2)).ToString();
    }

    public string SolvePart2()
    {
        var image = MergeLayers(_layers);
        return DrawImage(image);
    }

    private static int[] MergeLayers(Layer[] layers)
    {
        var image = new int[WIDTH * HEIGHT];

        for (int i = 0; i < image.Length; i++)
        {
            image[i] = layers.FirstOrDefault(layer => layer.Data[i] != 2)?.Data[i] ?? 2;
        }

        return image;
    }

    public static string DrawImage(int[] image)
    {
        StringBuilder sb = new();
        sb.AppendLine();

        for (int i = 0; i < image.Length;)
        {
            for (int k = 0; k < WIDTH; k++, i++)
                sb.Append(C2S(image[i]));

            sb.AppendLine();
        }

        return sb.ToString();
    }

    public static char C2S(int c) => c switch
    {
        0 => '.',
        1 => '#',
        _ => ' '
    };
}
