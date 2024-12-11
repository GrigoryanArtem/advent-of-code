namespace Puzzles.Base;
public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((d, i) => (d, i));

    public static ulong UInt64Sum<T>(this IEnumerable<T> source, Func<T, ulong> func)
        => source.Aggregate(0UL, (acc, n) => acc + func(n));
    

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);

        return source;
    }
}
