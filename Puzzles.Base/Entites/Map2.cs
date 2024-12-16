using System.Collections;

namespace Puzzles.Base.Entites;
public class Map2<T> : IEnumerable<T>
{
    public const int UP = 0;
    public const int RIGHT = 1;
    public const int DOWN = 2;
    public const int LEFT = 3;

    public Map2(T[] data, int columns)
    {
        Columns = columns;
        Rows = data.Length / columns;
        Data = data;
        Directions = [-Columns, 1, Columns, -1];
    }

    public T this[int loc]
    {
        get => Data[loc];
        set => Data[loc] = value;
    }

    public T this[int x, int y]
    {
        get => Data[D2toD1(x, y)];
        set => Data[D2toD1(x, y)] = value;
    }

    public int Rows { get; }
    public int Columns { get; }
    public T[] Data { get; }

    public int[] Directions { get; }

    public int Next(int location, int direction)
        => location + Directions[direction];

    public Map2<T> Copy()
    {
        var buffer = new T[Data.Length];
        Array.Copy(Data, buffer, Data.Length);

        return new(buffer, Columns);
    }

    public void Draw(Func<T, int, string>? formatter = null)
    {
        var f = formatter ?? DefaultFormat;

        for (int i = 0; i < Data.Length; i += Columns)
        {
            for (int k = i; k < i + Columns; k++)
                Console.Write(f(Data[k], k));

            Console.WriteLine();
        }
    }

    public (int location, int direction) LD2L(int ld)
        => (ld / Directions.Length, ld % Directions.Length);

    public int L2LD(int location, int direction)
        => Directions.Length * location + direction;

    public int D2toD1(int x, int y)
        => y * Columns + x;

    public (int x, int y) D1toD2(int loc)
        => (loc % Columns, loc / Columns);

    public IEnumerator<T> GetEnumerator()
        => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Data.GetEnumerator();

    #region Private methods

    public string DefaultFormat(T value, int location)
        => value?.ToString() ?? String.Empty;

    #endregion
}
