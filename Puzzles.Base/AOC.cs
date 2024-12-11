namespace Puzzles.Base;
public static class AOC
{
    private static readonly ulong[] digitsDividers;

    static AOC()
        => digitsDividers = InitDividers();

    public static int GetDigits(ulong number)
        => (int)(Math.Log10(number) + 1);

    public static (ulong left, ulong right) SplitUInt64(ulong number, int size)
        => (number / digitsDividers[size], number % digitsDividers[size]);

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
