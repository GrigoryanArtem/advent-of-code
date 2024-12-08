namespace Puzzles.Base.Entites;

public record Point2(int X, int Y)
{
    public static Point2 operator +(Point2 a, Point2 b)
        => new(a.X + b.X, a.Y + b.Y);

    public static Point2 operator *(Point2 a, int b)
       => new(a.X * b, a.Y * b);

    public static Point2 operator -(Point2 a, Point2 b)
        => new(a.X - b.X, a.Y - b.Y);

    public void Deconstruct(out int x, out int y)
    {
        x = X;
        y = Y;
    }
}
