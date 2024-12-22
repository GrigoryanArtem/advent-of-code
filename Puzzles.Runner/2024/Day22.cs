using System.Collections.Concurrent;

namespace Puzzles.Runner._2024;

[Puzzle("Monkey Market", 22, 2024)]
public class Day22(ILinesInputReader input) : IPuzzleSolver
{    
    #region Constants

    private const long MOD = 16777216L;
    private const long SEQ_SIZE = 4;
    private const int ITERATIONS = 2000;
    private const int BASE_10 = 10;

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
        var sums = new ConcurrentDictionary<int, long>();
        _numbers.AsParallel().ForAll(n => CalculateSeqs(n, ITERATIONS, sums));            
        return sums.Values.AsParallel().Max().ToString();
    }

    #region Private methods

    private static long CalculateSecret(long secret, int iterations)
    {
        for (int i = 0; i < iterations; i++)
            secret = NextSecret(secret);

        return secret;
    }

    private static void CalculateSeqs(long secret, long iterations, ConcurrentDictionary<int, long> sums)
    {
        var added = new HashSet<int>();
        iterations -= SEQ_SIZE;

        var seq = 0;                
        var prev = 0L;

        void update()
        {
            var digit = secret % BASE_10;
            seq = (seq << 8) + (byte)(digit - prev + BASE_10);
            secret = NextSecret(secret);
            prev = digit;
        }
        
        for (int i = 0; i < SEQ_SIZE; i++)        
            update();

        for (int i = 0; i < iterations; i++)
        {
            update();

            if (!added.Contains(seq))
            {
                sums.AddOrUpdate(seq, prev, (_, v) => v + prev);
                added.Add(seq);
            }
        }
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
