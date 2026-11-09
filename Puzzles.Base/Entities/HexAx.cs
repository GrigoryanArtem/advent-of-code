namespace Puzzles.Base.Entities;

public record struct HexAx
{
    public int Q { get; set; }
    public int R { get; set; }
    public readonly int S => -Q - R;

    public HexAx() { }
    public HexAx(int q, int r)
    {
        Q = q;
        R = r;
    }

    public readonly void Deconstruct(out int q, out int r)
    {
        q = Q;
        r = R;
    }

    public void Add(HexAx d)
    {
        Q += d.Q;
        R += d.R;
    }

    public void Subtract(HexAx d)
    {
        Q -= d.Q;
        R -= d.R;
    }

    public readonly int HexLength()
        => AOC.Max3(Math.Abs(Q), Math.Abs(R), Math.Abs(S));

    public readonly int DistanceTo(HexAx pos)
       => Distance(this, pos);

    public static HexAx operator -(HexAx a, HexAx b)
        => new(a.Q - b.Q, a.R - b.R);

    public static HexAx operator +(HexAx a, HexAx b)
        => new(a.Q + b.Q, a.R + b.R);

    public static int Distance(HexAx a, HexAx b)
        => (a - b).HexLength();

    public static HexAx Zero => new(0, 0);

    public override readonly string ToString()
        => $"{Q} {R} {S}";
}
