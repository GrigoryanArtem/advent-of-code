namespace Puzzles.Runner.Base;

public class ConsoleHistogram(int width, int height, int margin, string? unit)
{
    private const char BAR_CHAR = '|';
    private const char EMPTY_CHAR = ' ';
    private const char AXIS_LINE_CHAR = '-';
    private const char AXIS_TICK_CHAR = 'x';

    private const int Y_AXIS_LABEL_WIDTH = 6;
    private readonly string AXIS_PADDING = new(' ', 9);

    private readonly string _unit = unit ?? string.Empty;

    public void Draw(double[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var (min, max) = GetRange(data);
        if (IsDegenerate(min, max))
            return;

        var buckets = BuildBuckets(data, min, max);
        var maxCount = buckets.Max();

        var leftMargin = CreateLeftMargin();

        DrawBars(buckets, maxCount, leftMargin);
        DrawAxisLine(leftMargin);
        DrawAxisLabels(min, max, leftMargin);
    }

    private static bool IsDegenerate(double min, double max)
        => Math.Abs(max - min) < double.Epsilon;

    private static (double min, double max) GetRange(double[] sorted)
    {
        var min = Percentile(sorted, 0.01);
        var max = Percentile(sorted, 0.99);

        return (min, max);
    }

    private int[] BuildBuckets(double[] data, double min, double max)
    {
        var buckets = new int[width];

        foreach (var v in data)
        {
            if (v < min || v > max)
                continue;

            var idx = (int)((v - min) / (max - min) * (width - 1));
            idx = Math.Clamp(idx, 0, width - 1);

            buckets[idx]++;
        }

        return buckets;
    }

    private string CreateLeftMargin()
        => new(' ', margin);

    private void DrawBars(int[] buckets, int maxCount, string leftMargin)
    {
        for (int row = height; row >= 1; row--)
        {
            var yValue = (int)Math.Round((double)row / height * maxCount);

            Console.Write(leftMargin);
            Console.Write($"{yValue,Y_AXIS_LABEL_WIDTH} | ");

            for (int col = 0; col < width; col++)
            {
                var barHeight = (double)buckets[col] / maxCount * height;
                Console.Write(barHeight >= row ? BAR_CHAR : EMPTY_CHAR);
            }

            Console.WriteLine();
        }
    }

    private void DrawAxisLine(string leftMargin)
    {
        Console.Write(leftMargin);
        Console.Write("       +");

        var ticks = GetTickPositions();

        for (int i = 0; i < width; i++)
            Console.Write(ticks.Contains(i) ? AXIS_TICK_CHAR : AXIS_LINE_CHAR);


        Console.WriteLine();
    }

    private void DrawAxisLabels(double min, double max, string leftMargin)
    {
        Console.Write(leftMargin);
        Console.Write(AXIS_PADDING);

        var ticks = GetTickPositions();

        int cursor = 0;

        for (int i = 0; i < ticks.Length; i++)
        {
            int pos = ticks[i];

            double value = min + (double)pos / (width - 1) * (max - min);

            string label = FormatLabel(value, i == ticks.Length - 1);

            int start = pos - label.Length / 2;
            int spaces = start - cursor;

            if (spaces > 0)
            {
                Console.Write(new string(' ', spaces));
                cursor += spaces;
            }

            Console.Write(label);
            cursor += label.Length;
        }

        Console.WriteLine();
    }

    private int[] GetTickPositions() =>
    [
        1,
        width / 4,
        width / 2,
        width * 3 / 4,
        width - 1
    ];

    private string FormatLabel(double value, bool withUnit)
        => withUnit
            ? $"{value:F3} {_unit}"
            : $"{value:F3}";

    private static double Percentile(double[] sorted, double p)
    {
        var index = (sorted.Length - 1) * p;

        var lower = (int)Math.Floor(index);
        var upper = (int)Math.Ceiling(index);

        if (lower == upper)
            return sorted[lower];

        return sorted[lower] +
               (index - lower) *
               (sorted[upper] - sorted[lower]);
    }
}