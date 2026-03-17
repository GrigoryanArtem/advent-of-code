namespace Puzzles.Runner.Base.Histogram;

public class MermaidHistogram(int width, string? unit)
    : HistogramBase(width, unit)
{
    public void DrawToFile(string filePath, double[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var sorted = data.OrderBy(x => x).ToArray();

        var (min, max) = GetRange(sorted, .95);
        if (IsDegenerate(min, max))
            return;

        var buckets = BuildBuckets(sorted, min, max);
        var content = BuildMermaid(buckets, min, max);

        File.WriteAllText(filePath, content);
    }

    public void DrawMarkdownToFile(string filePath, double[] data)
    {
        if (data == null || data.Length == 0)
            return;

        var sorted = data.OrderBy(x => x).ToArray();

        var (min, max) = GetRange(sorted, .8);
        if (IsDegenerate(min, max))
            return;

        var buckets = BuildBuckets(sorted, min, max);
        var mermaid = BuildMermaid(buckets, min, max);

        var markdown = $$"""
        ```mermaid
        {{mermaid}}
        ```
        """;

        File.WriteAllText(filePath, markdown);
    }

    protected override void Render(double[] sorted, int[] buckets, double min, double max)
    {
    }

    private string BuildMermaid(int[] buckets, double min, double max)
    {
        var labels = BuildXLabels(min, max);

        return $$"""

        xychart-beta
        title "Histogram"
        x-axis [{{string.Join(", ", labels)}}]
        y-axis "count" 0 --> {{Math.Max(1, buckets.Max())}}
        bar [{{string.Join(", ", buckets)}}]
        """;
    }

    private string[] BuildXLabels(double min, double max)
    {
        var labels = new string[Width];

        for (int i = 0; i < Width; i++)
        {
            var value = min + (double)i / (Width - 1) * (max - min);

            labels[i] = $"\"{FormatLabel(value, false)}\"";
        }

        return labels;
    }

}