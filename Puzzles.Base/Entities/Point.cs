namespace Puzzles.Base.Entities;

public record Point(int X, int Y)
{
    public static Point operator +(Point a, Point b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Point operator *(Point a, int b)
       => new(a.X * b, a.Y * b);

    public static Point operator -(Point a, Point b)
        => new(a.X - b.X, a.Y - b.Y);

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }
}
