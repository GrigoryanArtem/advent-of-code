namespace Puzzles.Runner._2024;

[Puzzle("Monkey Market", 22, 2024)]
public class Day22(ILinesInputReader input) : IPuzzleSolver
{
    private const ulong MOD = 16777216UL;
    
    public ulong[] _numbers = [];

    public void Init()
        => _numbers = input.Convert(Convert.ToUInt64);

    public string SolvePart1()
        => _numbers.AsParallel()
            .Select(n => CalculateSecret(n, 2000))
            .UInt64Sum(s => s)
            .ToString();

    private static ulong CalculateSecret(ulong secret, int iterations)
    {
        for (int i = 0; i < 2000; i++)
            secret = NextSecret(secret);

        return secret;
    }

    private static ulong NextSecret(ulong secret)
    {
        var step1 = Prune(Mix(secret, secret << 6));
        var step2 = Prune(Mix(step1, step1 >> 5));
        var step3 = Prune(Mix(step2, step2 << 11));

        return step3;
    }

    private static ulong Mix(ulong secret, ulong num)
        => secret ^ num;

    private static ulong Prune(ulong secret)
        => secret % MOD;
}
