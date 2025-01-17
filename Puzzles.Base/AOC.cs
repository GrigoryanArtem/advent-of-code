using Puzzles.Base.Entites;

namespace Puzzles.Base;
public static class AOC
{
    #region Constants

    public const double PI2 = Math.PI * 2;
    public const double HALF_PI = Math.PI / 2;

    #endregion

    private static readonly ulong[] digitsDividers;

    static AOC()
        => digitsDividers = InitDividers();

    #region Directions

    public static Vec2[] Directions2D =>
    [
        new Vec2(0, 1),  // UP
        new Vec2(1, 0),  // RIGHT
        new Vec2(0, -1), // DOWN
        new Vec2(-1, 0)  // LEFT
    ];

    public static Vec2[] Directions2DInv =>
    [
        new Vec2(0, -1), // UP
        new Vec2(1, 0),  // RIGHT
        new Vec2(0, 1),  // DOWN
        new Vec2(-1, 0)  // LEFT
    ];

    #endregion

    #region Math

    public static int GetDigits(ulong number)
       => (int)(Math.Log10(number) + 1);

    public static (ulong left, ulong right) SplitUInt64(ulong number, int size)
        => (number / digitsDividers[size], number % digitsDividers[size]);

    public static int Mod(int n, int m)
        => ((n % m) + m) % m;

    public static int ModInv(int n, int m)
    {
        int t = 0, nt = 1;
        int r = m, nr = n;

        while (nr != 0)
        {
            int q = r / nr;
            (r, nr) = (nr, r - q * nr);
            (t, nt) = (nt, t - q * nt);
        }

        return Mod(t, m);
    }

    #endregion

    #region Sort

    public static void Sort2(ref int a, ref int b)
    {
        if (a > b)
            (b, a) = (a, b);
    }

    public static void Sort3(ref int a, ref int b, ref int c)
    {
        Sort2(ref a, ref b);
        Sort2(ref a, ref c);
        Sort2(ref b, ref c);
    }

    #endregion

    #region Vectors

    public static double Angle(Vec2 from, Vec2 to)
        => Angle(from.X, from.Y, to.X, to.Y);
    public static double Angle(int ax, int ay, int bx, int by)
        => Math.Atan2(by - ay, bx - ax);

    public static double EuclideanDistance(Vec2 from, Vec2 to)
        => EuclideanDistance(from.X, from.Y, to.X, to.Y);

    public static double EuclideanDistance(int ax, int ay, int bx, int by)
        => Math.Sqrt((ax - bx) * (ax - bx) + (ay - by) * (ay - by));

    public static int ManhattanDistance(int ax, int ay, int bx, int by)
        => Math.Abs(ax - bx) + Math.Abs(ay - by);

    public static int ManhattanDistance(Vec2 from, Vec2 to)
        => ManhattanDistance(from.X, from.Y, to.X, to.Y);

    public static int ManhattanDistance(int ax, int ay, int az, int bx, int by, int bz)
        => Math.Abs(ax - bx) + Math.Abs(ay - by) + Math.Abs(az - bz);

    public static int ManhattanDistance(Vec3 from, Vec3 to)
        => ManhattanDistance(from.X, from.Y, from.Z, to.X, to.Y, to.Z);

    #endregion

    #region Private methods

    private static ulong[] InitDividers()
    {
        var ulongDigits = GetDigits(ulong.MaxValue);
        var digitsDividers = new ulong[ulongDigits];

        digitsDividers[0] = 0UL;
        digitsDividers[1] = 10UL;

        for (int i = 2; i < ulongDigits; i++)
            digitsDividers[i] = digitsDividers[i - 1] * 10UL;

        return digitsDividers;
    }

    #endregion
}
