namespace Puzzles.Base.Entities;

public readonly record struct Color
{
    public byte R { get; init; }
    public byte G { get; init; }
    public byte B { get; init; }

    public Color() { }
    public Color(byte c) : this(c, c, c) { }
    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }
}
