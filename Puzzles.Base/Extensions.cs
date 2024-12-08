namespace Puzzles.Base;
public static class Extensions
{
    public static IEnumerable<(T, int)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((d, i) => (item: d, index: i));
}
