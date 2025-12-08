using System.Collections;

namespace Puzzles.Base.Entities;
public class Mat2<T> : IEnumerable<T>
{
    public const int UP = 0;
    public const int RIGHT = 1;
    public const int DOWN = 2;
    public const int LEFT = 3;

    private Mat2()
    {
        Columns = Rows = 0;
        Data = [];
        Directions = [0, 0, 0, 0];
    }

    public Mat2(T[] data, int columns)
    {
        Columns = columns;
        Rows = data.Length / columns;
        Data = data;
        Directions = [-Columns, 1, Columns, -1];
    }

    public ref T Ref(int loc)
        => ref Data[loc];

    public ref T Ref(int x, int y)
        => ref Data[D2toD1(x, y)];

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

    public int InvDdx(int ddx)
        => (ddx + 2) % Directions.Length;

    public int Next(int location, int ddx)
        => location + Directions[ddx];

    public void FillBorders(T value)
    {
        for (int x = 0; x < Columns; x++)
            this[x, 0] = this[x, Rows - 1] = value;

        for (int y = 0; y < Rows; y++)
            this[0, y] = this[Columns - 1, y] = value;
    }

    public TBuffer[] CreateBuffer<TBuffer>(TBuffer defaultValue = default!)
    {
        var buffer = new TBuffer[Data.Length];
        Array.Fill(buffer, defaultValue);
        return buffer;
    }

    public Mat2<T> Copy()
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

    public int Manhattan(int from, int to)
    {
        var (cx, cy) = D1toD2(from);
        var (ex, ey) = D1toD2(to);

        return Math.Abs(cx - ex) + Math.Abs(cy - ey);
    }

    public IEnumerator<T> GetEnumerator()
        => Data.AsEnumerable().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => Data.GetEnumerator();

    public IEnumerable<T> Row(int row)
    {
        var start = D2toD1(0, row);
        for(int i = 0; i < Columns; i++)
            yield return this[start + i];
    }

    public Span<T> RowSpan(int row)
    {
        var start = D2toD1(0, row);
        return Data.AsSpan()[start..(start+Columns)];
    }

    public IEnumerable<T> Column(int column)
    {
        var start = D2toD1(column, 0);
        for (int i = 0; i < Rows; i++)
            yield return this[start + i * Columns];
    }

    public static Mat2<T> Null => new();

    public static Mat2<T> Empty(int columns, int rows)
        => new(new T[rows * columns], columns);

    public static Mat2<T> Fill(int columns, int rows, T value)
    {
        var mat = new Mat2<T>(new T[rows * columns], columns);
        Array.Fill(mat.Data, value);

        return mat;
    }

    public static Mat2<T> WithBorders(T[] data, int columns, T borderValue)
    {
        var rows = data.Length / columns;

        var map = Empty(columns + 2, rows + 2);
        map.FillBorders(borderValue);

        for (int y = 0; y < rows; y++)
            for (int x = 0; x < columns; x++)
                map[x + 1, y + 1] = data[y * columns + x];

        return map;
    }

    #region Private methods

    private string DefaultFormat(T value, int location)
        => value?.ToString() ?? String.Empty;

    #endregion
}
