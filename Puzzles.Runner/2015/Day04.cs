using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Puzzles.Runner._2015;

[Puzzle("The Ideal Stocking Stuffer", 4, 2015)]
public class Day04(IFullInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => Solve(input.Text, 5).ToString();

    public string SolvePart2()
        => Solve(input.Text, 6).ToString();

    private static uint Solve(string key, int targetCount)
    {
        var keyBytes = Encoding.ASCII.GetBytes(key);
        var threads = Environment.ProcessorCount;
        var best = uint.MaxValue;

        Parallel.For(0, threads, (threadId, state) =>
        {
            Span<byte> data = stackalloc byte[keyBytes.Length + 10];
            Span<byte> hash = stackalloc byte[16];

            keyBytes.CopyTo(data);

            for (uint i = (uint)threadId; !state.IsStopped && i < Volatile.Read(ref best); i += (uint)threads)
            {
                Utf8Formatter.TryFormat(i, data[keyBytes.Length..], out int len);
                MD5.TryHashData(data[..(keyBytes.Length + len)], hash, out _);

                if (HasLeadingHexZeros(hash, targetCount))
                {
                    AtomicMin(ref best, i);
                    state.Stop();

                    break;
                }
            }
        });

        return best;
    }

    private static void AtomicMin(ref uint location, uint value)
    {
        uint current;

        while (value < (current = location))
            Interlocked.CompareExchange(ref location, value, current);
    }

    private static bool HasLeadingHexZeros(ReadOnlySpan<byte> hash, int hexZeros)
    {
        var full = hexZeros >> 1;
        var half = (hexZeros & 1) != 0;

        var success = true;
        for (int i = 0; success && i < full; i++)
            success &= hash[i] == 0;

        return success && (!half || (hash[full] & 0xF0) == 0);
    }
}
