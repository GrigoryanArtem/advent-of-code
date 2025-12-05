using Puzzles.Base.Entities;
using System.Text;

namespace Puzzles.Base;

public static class Mat2Extensions
{
    public static void ToPPM<T>(this Mat2<T> mat, string output, Func<T, Color> converter, int scale = 1)
    {
        using var writer = new FileStream(output, FileMode.Create, FileAccess.Write, FileShare.None);

        int width = mat.Columns * scale;
        int height = mat.Rows * scale;

        var header = Encoding.ASCII.GetBytes($"P6\n{width} {height}\n255\n");
        writer.Write(header);

        Span<byte> rgb = stackalloc byte[3];
        for (int y = 0; y < mat.Rows; y++)
        {
            for (int sy = 0; sy < scale; sy++)
            {
                for (int x = 0; x < mat.Columns; x++)
                {
                    var color = converter(mat.Ref(x, y));
                    rgb[0] = color.R;
                    rgb[1] = color.G;
                    rgb[2] = color.B;

                    for (int sx = 0; sx < scale; sx++)
                        writer.Write(rgb);
                }
            }
        }
    }
}
