using Puzzles.Runner.Base;
using System.Text;

namespace Puzzles.Runner._2017;

[Puzzle("Knot Hash", 10, 2017)]
public class Day10(IFullInputReader input, IRunInfo runInfo) : IPuzzleSolver
{
    private const int DENSE_BLOCKS  = 16;    
    private const int DENSE_BLOCK  = 16;   

    private int[] _hashLengths = [];

    public void Init()
    {
        var bytes = Encoding.UTF8.GetBytes(input.Text);
        _hashLengths = new int[bytes.Length + 5];

        for (int i = 0; i < bytes.Length; i++)
            _hashLengths[i] = bytes[i];

        _hashLengths[^5] = 17;
        _hashLengths[^4] = 31;
        _hashLengths[^3] = 73;
        _hashLengths[^2] = 47;
        _hashLengths[^1] = 23;
    }

    public string SolvePart1()
    {
        var lengths = input.Text.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Select(Int32.Parse)
            .ToArray();

        Span<byte> buffer = stackalloc byte[runInfo.IsExample ? 5 : 256];

        RunRounds(buffer, lengths, 1);

        return (buffer[0] * buffer[1]).ToString();
    }

    public string SolvePart2()
    {
        Span<byte> buffer = stackalloc byte[256];

        RunRounds(buffer, _hashLengths, 64);
        return CalcualteDenseHash(buffer);
    }

    private string CalcualteDenseHash(Span<byte> buffer)
    {
        Span<byte> denseHash = stackalloc byte[16];

        for (int i = 0; i < DENSE_BLOCKS; i++)
        {
            var hash = 0;
            for (int k = 0; k < DENSE_BLOCK; k++)
                hash ^= buffer[i * DENSE_BLOCK + k];

            denseHash[i] = (byte)hash;
        }

        return Convert.ToHexString(denseHash).ToLower();
    }

    private static void RunRounds(Span<byte> buffer, ReadOnlySpan<int> lengths, int rounds)
    {
        var n = buffer.Length;

        for (int i = 0; i < n; i++)
            buffer[i] = (byte)i;

        var pos = 0;
        var skip = 0;

        for (int r = 0; r < rounds; r++)
        {
            foreach (int length in lengths)
            {
                var lo = pos;
                var hi = pos + length - 1;

                if (hi >= n)
                    hi -= n;

                for (int i = 0; i < length / 2; i++)
                {
                    (buffer[lo], buffer[hi]) = (buffer[hi], buffer[lo]);

                    if (++lo == n)
                        lo = 0;

                    if (--hi < 0)
                        hi = n - 1;
                }

                pos = (pos + length + skip) % n;
                skip++;
            }
        }
    }
}
