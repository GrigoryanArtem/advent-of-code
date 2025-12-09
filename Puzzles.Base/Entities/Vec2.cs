using System.Collections;

namespace Puzzles.Base.Entities;

public record struct Vec2 : IEnumerable<int>
{
    public Vec2() { }

    public Vec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }

    public readonly void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }

    public readonly IEnumerator<int> GetEnumerator()
    {
        yield return X;
        yield return Y;
    }    

    public void Move(Vec2 dir)
    {
        X += dir.X;
        Y += dir.Y;
    }

    public readonly Vec2 Clamp(int val)
        => new(Math.Clamp(X, -val, val), Math.Clamp(Y, -val, val));

    public readonly Vec2 Abs()
        => new(Math.Abs(X), Math.Abs(Y));

    readonly IEnumerator IEnumerable.GetEnumerator() 
        => GetEnumerator();

    public static Vec2 operator -(Vec2 a, Vec2 b)
        => new(a.X - b.X, a.Y - b.Y);

    public static Vec2 operator +(Vec2 a, Vec2 b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Vec2 operator *(Vec2 a, int b)
        => new(a.X * b, a.Y * b);

    public static Vec2 Zero => new(0, 0);
}
