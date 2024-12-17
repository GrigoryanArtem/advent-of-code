using System.Reflection.Metadata.Ecma335;
using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Chronospatial Computer", 17, 2024)]
public partial class Day17(ILinesInputReader input) : IPuzzleSolver
{
    private const int PROGRAM_INDEX = 4;
    private const int NUMBER_OF_REGISTERS = 3;

    private const int A = 0;
    private const int B = 1;
    private const int C = 2;

    private const int ADV = 0;
    private const int BXL = 1;
    private const int BST = 2;
    private const int JNZ = 3;
    private const int BXC = 4;
    private const int OUT = 5;
    private const int BDV = 6;
    private const int CDV = 7;


    public long[] _init = [];

    public long[] _r = new long[NUMBER_OF_REGISTERS];
    public byte[] _p = [];
    public List<byte> _out = [];

    public void Init()
    {
        _init = Enumerable.Range(0, NUMBER_OF_REGISTERS)
            .Select(i => Convert.ToInt64(NumRegex()
                .Match(input.Lines[i]).Value))
            .ToArray();

        _p = NumRegex().Matches(input.Lines[PROGRAM_INDEX])
            .Select(m => Convert.ToByte(m.Value))
            .ToArray();
    }

    public string SolvePart1()
    {
        ResetState();

        for (int op = 0; op < _p.Length;)
        {
            var opcode = _p[op];
            op = RunOp(op, _p[op + 1]);
        }

        return String.Join(",", _out);
    }

    private int RunOp(int op, int literal) => _p[op] switch
    {
        ADV => op + Dv(_r[A], Combo(literal), ref _r[A]),
        BDV => op + Dv(_r[A], Combo(literal), ref _r[B]),
        CDV => op + Dv(_r[A], Combo(literal), ref _r[C]),

        BST => op + Mod(Combo(literal), ref _r[B]),

        BXL => op + Xor(_r[B], literal, ref _r[B]),
        BXC => op + Xor(_r[B], _r[C], ref _r[B]),

        JNZ => Jmp(op, literal),
        OUT => op + Out(Combo(literal)),

        _ => throw new NotImplementedException()
    };

    private int Out(long literal)
    {
        _out.Add((byte)(literal % 8));
        return 2;
    }
    private int Jmp(int op, int literal)
        => _r[A] == 0 ? op + 2 : literal;

    private int Mod(long a, ref long output)
    {
        output = a % 8;
        return 2;
    }

    private int Xor(long a, long b, ref long output)
    {
        output = a ^ b;
        return 2;
    }

    private int Dv(long a, long b, ref long output)
    {
        output = (long)(a / Math.Pow(2, b));
        return 2;
    }

    private long Combo(int n) => n switch
    {
        >=4 => _r[n - 4],
        _ => n
    };

    private void ResetState()
    {
        Array.Copy(_init, _r, _init.Length);
        _out.Clear();
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumRegex();
}
