namespace Puzzles.Runner._2024;

[Puzzle("Monkey Market", 22, 2024)]
public class Day22(ILinesInputReader input) : IPuzzleSolver
{    
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

    private static Dictionary<int, long> CalculateSeqs(long secret, long iterations)
    {
        Dictionary<int, long> result = [];

        var buffer = new int[SEQ_SIZE];
        var tail = 0L;
        var prev = 0L;

        long updateBuffer()
        {
            var digit = secret % 10;
            buffer[tail] = (int)(digit - prev) + 10;

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

    private static int B2S(int[] buffer, long tail)
    {
        var answer = 0;

        for (int i = 1; i <= SEQ_SIZE; i++)
            answer = (answer << 5) + buffer[(tail + i) % SEQ_SIZE];

        return answer;
    }

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
