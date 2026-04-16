using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;

namespace Puzzles.Runner._2016;

[Puzzle("How About a Nice Game of Chess?", 5, 2016)]
public partial class Day05(IFullInputReader input) : IPuzzleSolver
{
    private static readonly char[] HEX =
    [
        '0', '1', '2', '3', '4', '5',
        '6', '7', '8', '9', 'a', 'b',
        'c', 'd', 'e', 'f'
    ];

    public string SolvePart1()
    {
        Span<char> pass = stackalloc char[8];
        var pidx = 0;

        var keyBytes = Encoding.ASCII.GetBytes(input.Text);

        Span<byte> data = stackalloc byte[keyBytes.Length + 10];
        Span<byte> hash = stackalloc byte[16];

        keyBytes.CopyTo(data);

        for (uint i = 0; pidx < pass.Length; i++)
        {
            Utf8Formatter.TryFormat(i, data[keyBytes.Length..], out int len);
            MD5.TryHashData(data[..(keyBytes.Length + len)], hash, out _);

            if (HasLeadingHexZeros(hash))            
                pass[pidx++] = HEX[hash[2]];            
        }

        return new string(pass);
    }

    public string SolvePart2()
    {
        Span<char> pass = stackalloc char[8];
        var pidx = 0;

        var keyBytes = Encoding.ASCII.GetBytes(input.Text);                

        Span<byte> data = stackalloc byte[keyBytes.Length + 10];
        Span<byte> hash = stackalloc byte[16];

        keyBytes.CopyTo(data);

        for (uint i = 0; pidx < pass.Length; i++)
        {
            Utf8Formatter.TryFormat(i, data[keyBytes.Length..], out int len);
            MD5.TryHashData(data[..(keyBytes.Length + len)], hash, out _);

            if (HasLeadingHexZeros(hash))
            {
                var pos = hash[2];
                if (pos < pass.Length && pass[pos] == 0)
                {
                    pass[pos] = HEX[hash[3] >> 4];
                    pidx++;
                }
            }
        }

        return new string(pass);
    }

    private static bool HasLeadingHexZeros(ReadOnlySpan<byte> hash)
        => hash[0] == 0
            && hash[1] == 0
            && (hash[2] & 0xF0) == 0;
}