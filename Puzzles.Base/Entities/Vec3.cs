using System.Collections;

namespace Puzzles.Base.Entities;

public record struct Vec3 : IEnumerable<int>
{
    public Vec3() { }

    public Vec3(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public readonly void Deconstruct(out int x, out int y, out int z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public readonly IEnumerator<int> GetEnumerator()
    {
        yield return X;
        yield return Y;
        yield return Z;
    }

    readonly IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public static Vec3 operator -(Vec3 a, Vec3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vec3 operator +(Vec3 a, Vec3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vec3 Zero => new(0, 0, 0);
}
