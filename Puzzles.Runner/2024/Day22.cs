namespace Puzzles.Runner._2024;

[Puzzle("Monkey Market", 22, 2024)]
public class Day22(ILinesInputReader input) : IPuzzleSolver
{
    private readonly record struct Seq(long N1, long N2, long N3, long N4);

    #region Constants

    private const long MOD = 16777216L;
    private const long SEQ_SIZE = 4;
    private const int ITERATIONS = 2000;

    #endregion

    private long[] _numbers = [];

    public void Init()
        => _numbers = input.Convert(Convert.ToInt64);

    public string SolvePart1()
        => _numbers.AsParallel()
            .Sum(s => CalculateSecret(s, ITERATIONS))
            .ToString();

    public string SolvePart2()
    {
        var diffs = _numbers.AsParallel()
            .Select(n => CalculateSeqs(n, ITERATIONS))
            .ToArray();

        var seqs = diffs
            .SelectMany(d => d.Keys)
            .Distinct()
            .ToArray();

        return seqs.AsParallel()
            .Select(k => diffs.Sum(d => d.GetValueOrDefault(k, 0)))
            .OrderByDescending(s => s)
            .First()
            .ToString();
    }

    #region Private methods

    private static long CalculateSecret(long secret, int iterations)
    {
        for (int i = 0; i < iterations; i++)
            secret = NextSecret(secret);

        return secret;
    }

    private static Dictionary<Seq, long> CalculateSeqs(long secret, long iterations)
    {
        Dictionary<Seq, long> result = [];

        var buffer = new long[SEQ_SIZE];
        var tail = 0L;
        var prev = 0L;

        long updateBuffer()
        {
            var digit = secret % 10;
            buffer[tail] = digit - prev;

            return digit;
        }

        void next(long digit)
        {
            prev = digit;
            tail = (tail + 1L) % SEQ_SIZE;
            secret = NextSecret(secret);
        }

        for (int i = 0; i < SEQ_SIZE; i++)
            next(updateBuffer());

        iterations -= SEQ_SIZE;
        for (int i = 0; i < iterations; i++)
        {
            var digit = updateBuffer();

            var seq = B2S(buffer, tail);
            result.TryAdd(seq, digit);

            next(digit);
        }

        return result;
    }

    private static Seq B2S(long[] buffer, long tail) => new
    (
        N1: buffer[(tail + 1) % SEQ_SIZE],
        N2: buffer[(tail + 2) % SEQ_SIZE],
        N3: buffer[(tail + 3) % SEQ_SIZE],
        N4: buffer[(tail + 4) % SEQ_SIZE]
    );

    private static long NextSecret(long secret)
    {
        var step1 = MixAndPrune(secret, secret << 6);
        var step2 = MixAndPrune(step1, step1 >> 5);
        return MixAndPrune(step2, step2 << 11);
    }

    private static long MixAndPrune(long secret, long num)
        => (secret ^ num) % MOD;

    #endregion
}
