namespace Puzzles.Runner.Base.Histogram;

public class ConsoleHistogram(int width, int height, int margin, string? unit)
    : HistogramBase(width, unit)
{
    private const char BarChar = '|';
    private const char EmptyChar = ' ';
    private const char AxisLineChar = '-';
    private const char AxisTickChar = 'x';

    private const int YAxisLabelWidth = 6;

    private readonly int _height = height;
    private readonly int _margin = margin;
    private readonly string _axisPadding = new(' ', 9);

    protected override void Render(double[] sorted, int[] buckets, double min, double max)
    {
        var maxCount = buckets.Max();
        if (maxCount <= 0)
            return;

        var leftMargin = CreateLeftMargin();

        DrawBars(buckets, maxCount, leftMargin);
        DrawAxisLine(leftMargin);
        DrawAxisLabels(min, max, leftMargin);
    }

    private string CreateLeftMargin()
        => new(' ', _margin);

    private void DrawBars(int[] buckets, int maxCount, string leftMargin)
    {
        for (int row = _height; row >= 1; row--)
        {
            var yValue = (int)Math.Round((double)row / _height * maxCount);

            Console.Write(leftMargin);
            Console.Write($"{yValue,YAxisLabelWidth} | ");

            for (int col = 0; col < Width; col++)
            {
                var barHeight = (double)buckets[col] / maxCount * _height;
                Console.Write(barHeight >= row ? BarChar : EmptyChar);
            }

            Console.WriteLine();
        }
    }

    private void DrawAxisLine(string leftMargin)
    {
        Console.Write(leftMargin);
        Console.Write("       +");

        var ticks = GetTickPositions();
        var tickSet = new HashSet<int>(ticks);

        for (int i = 0; i < Width; i++)
            Console.Write(tickSet.Contains(i) ? AxisTickChar : AxisLineChar);

        Console.WriteLine();
    }

    private void DrawAxisLabels(double min, double max, string leftMargin)
    {
        Console.Write(leftMargin);
        Console.Write(_axisPadding);

        var ticks = GetTickPositions();
        var line = new char[Width];
        Array.Fill(line, ' ');

        for (int i = 0; i < ticks.Length; i++)
        {
            int pos = ticks[i];

            double value = min + (double)pos / (Width - 1) * (max - min);
            string label = FormatLabel(value, i == ticks.Length - 1);

            int start = pos - label.Length / 2;

            if (i == 0 && start < 0)
                start = 0;

            if (i == ticks.Length - 1 && start + label.Length > Width)
                start = Width - label.Length;

            start = Math.Max(0, start);

            for (int j = 0; j < label.Length; j++)
            {
                int idx = start + j;
                if (idx < 0 || idx >= line.Length)
                    continue;

                line[idx] = label[j];
            }
        }

        Console.WriteLine(new string(line));
    }
}