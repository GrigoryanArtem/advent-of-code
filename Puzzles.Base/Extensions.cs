namespace Puzzles.Base;
public static class Extensions
{
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> source)
        => source.Select((d, i) => (d, i));

    public static ulong UInt64Sum<T>(this IEnumerable<T> source, Func<T, ulong> func)
        => source.Aggregate(0UL, (acc, n) => acc + func(n));

    public static long Mul<T>(this IEnumerable<T> source, Func<T, int> func)
       => source.Aggregate(1L, (acc, n) => acc * func(n));

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> source)
        where T : class
        => source.Where(x => x is not null);

    public static IEnumerable<T> WhereNotNull<T, V>(this IEnumerable<T> source, Func<T, V> selector)
       where V : class
       => source.Where(x => selector(x) is not null);

    public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);

        return source;
    }

    public static (V min, V max) MinMax<T, V>(this Span<T> source, Func<T, V> selector)
        where V : IComparable<V>
    {
        if (source.Length == 0)
            throw new InvalidOperationException("Sequence contains no elements");

        var min = selector(source[0]);
        var max = selector(source[0]);

        foreach (var el in source)
        {
            var value = selector(el);

            if (value.CompareTo(min) < 0)
                min = value;

            if (value.CompareTo(max) > 0)
                max = value;
        }

        return (min, max);
    }

    public static (V min, V max) MinMax<T, V>(this IEnumerable<T> source, Func<T, V> selector)
        where V : IComparable<V>
    {
        if(!source.Any())
            throw new InvalidOperationException("Sequence contains no elements");

        var min = selector(source.First());
        var max = selector(source.First());

        foreach(var el in source)
        {
            var value = selector(el);

            if (value.CompareTo(min) < 0)
                min = value;

            if (value.CompareTo(max) > 0)
                max = value;
        }

        return (min, max);
    }

    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> comparator)
    {
        var counter = 0;
        foreach (var item in source)
        {
            counter++;
            if (comparator(item))
                break;
        }

        return counter;
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

    public static int IndexOfMax<T, V>(this Span<T> source, Func<T, V> selector)
        where V : IComparable<V>
    {
        int max = 0;
        for (int i = 1; i < source.Length; i++)
            if (selector(source[i]).CompareTo(selector(source[max])) > 0)
                max = i;

        return max;
    }

    public static int IndexOfMax<T>(this Span<T> source)
        where T : IComparable<T>
    {
        int max = 0;
        for (int i = 1; i < source.Length; i++)
            if (source[i].CompareTo(source[max]) > 0)
                max = i;

        return max;
    }

    public static long LCM(this IEnumerable<int> source)
    {
        var acc = 1L;
        foreach (var item in source)
            acc = AOC.LCM(acc, item);

        return acc;
    }

    public static TValue AddAndReturn<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
         where TKey : notnull
    {
        source.Add(key, value); 
        return value;
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
         where TKey : notnull
    {
        source.TryAdd(key, value);
        return source[key];
    }

    public static TValue GetOrAdd<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, Func<TValue> func)
         where TKey : notnull
    {
        source.TryAdd(key, func());
        return source[key];
    }
}
