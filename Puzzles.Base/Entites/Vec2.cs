namespace Puzzles.Base.Entites;

public record struct Vec2
{
    public Vec2() { }

    public Vec2(int x, int y)
    {
        X = x;
        Y = y;
    }

    public int X { get; set; }
    public int Y { get; set; }
}
