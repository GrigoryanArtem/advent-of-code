namespace Puzzles.Runner.Base;

public record PuzzleStatistic<TAnswer> : IPuzzleStatistic
{
    protected PuzzleStatistic() { }

    public double[] Data { get; init; } = [];
    public int Iterations { get; init; }
    public double Mean { get; init; }
    public double Median { get; init; }
    public double StdDev { get; init; }
    public double P90 { get; init; }
    public double P95 { get; init; }
    public double P99 { get; init; }
    public TAnswer? Answer { get; init; }

    public static PuzzleStatistic<TAnswer> Create(IEnumerable<double> timesData)
        => Create(timesData, default);

    public static PuzzleStatistic<TAnswer> Create(IEnumerable<double> timesData, TAnswer? answer)
    {
        var times = timesData.ToArray();
        times.Sort();

        var span = times.AsSpan();

        var p90 = Percentile(span, 0.90);
        var p95 = Percentile(span, 0.95);
        var p99 = Percentile(span, 0.99);

        var idx99 = PercentileIndex(span.Length, 0.99);
        var times99 = span[..(idx99 + 1)];

        var sum = 0D;
        foreach (var t in times99)
            sum += t;

        var mean = sum / times99.Length;
        var median = Percentile(times99, 0.5);

        sum = 0;
        foreach (var t in times99)
        {
            var tmp = t - mean;
            sum += tmp * tmp;
        }

        var variance = sum / times99.Length;
        var stdDev = Math.Sqrt(variance);

        return new()
        {
            Data = span.ToArray(),
            Mean = mean,
            Median = median,
            P90 = p90,
            P95 = p95,
            P99 = p99,
            StdDev = stdDev,
            Iterations = span.Length,
            Answer = answer
        };
    }

    private static int PercentileIndex(int n, double p)
        => Math.Clamp((int)Math.Ceiling(p * n) - 1, 0, n - 1);

    private static double Percentile(Span<double> data, double p)
        => data[PercentileIndex(data.Length, p)];
}

