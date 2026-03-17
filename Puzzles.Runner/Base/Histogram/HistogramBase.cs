namespace Puzzles.Runner.Base.Histogram;

public abstract class HistogramBase(int width, string? unit)
{
    protected int Width { get; } = width;
    protected string Unit { get; } = unit ?? string.Empty;

    public void Draw(double[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var sorted = data.OrderBy(x => x).ToArray();

        var (min, max) = GetRange(sorted, .95);
        if (IsDegenerate(min, max))
            return;

        var buckets = BuildBuckets(sorted, min, max);

        Render(sorted, buckets, min, max);
    }

    protected abstract void Render(double[] sorted, int[] buckets, double min, double max);

    protected static bool IsDegenerate(double min, double max)
        => Math.Abs(max - min) < double.Epsilon;

    protected virtual (double min, double max) GetRange(double[] sorted, double p)
    {
        var min = Percentile(sorted, 0.00);
        var max = Percentile(sorted, p);

        return (min, max);
    }

    protected int[] BuildBuckets(double[] data, double min, double max)
    {
        var buckets = new int[Width];

        foreach (var v in data)
        {
            if (v < min || v > max)
                continue;

            var idx = (int)((v - min) / (max - min) * (Width - 1));
            idx = Math.Clamp(idx, 0, Width - 1);

            buckets[idx]++;
        }

        return buckets;
    }

    protected int[] GetTickPositions() =>
    [
        1,
        Width / 4,
        Width / 2,
        Width * 3 / 4,
        Width - 1
    ];

    protected string FormatLabel(double value, bool withUnit)
        => withUnit && !string.IsNullOrWhiteSpace(Unit)
            ? $"{value:F3} {Unit}"
            : $"{value:F3}";

    protected static double Percentile(double[] sorted, double p)
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