using System.Collections.Concurrent;

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

    public static int IndexOfMin<T, V>(this T[] source, Func<T, V> selector)
        where V : IComparable<V>
    {
        int min = 0;
        for(int i = 1; i < source.Length; i++)
            if (selector(source[i]).CompareTo(selector(source[min])) < 0)
                min = i;

        return min;
    }

    public static int IndexOfMax<T, V>(this T[] source, Func<T, V> selector)
        where V : IComparable<V>
    {
        int max = 0;
        for (int i = 1; i < source.Length; i++)
            if (selector(source[i]).CompareTo(selector(source[max])) > 0)
                max = i;

        return max;
    }

    public static TValue AddAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
    {
        source.Add(key, value); 
        return value;
    }
}
