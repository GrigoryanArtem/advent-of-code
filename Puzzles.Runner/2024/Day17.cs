using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Chronospatial Computer", 17, 2024)]
public partial class Day17(ILinesInputReader input) : IPuzzleSolver
{
    private delegate void Instruction(long a, long b, ref long output);

    private enum Code
    {
        ADV = 0,
        BXL = 1,
        BST = 2,
        JNZ = 3,
        BXC = 4,
        OUT = 5,
        BDV = 6,
        CDV = 7,
    }

    #region Constants

    private const int PROGRAM_INDEX = 4;
    private const int NUMBER_OF_REGISTERS = 3;

    private const int A = 0;
    private const int B = 1;
    private const int C = 2;    

    #endregion

    public long[] _init = [];    
    public byte[] _p = [];    

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
        long[] registers = new long[NUMBER_OF_REGISTERS];
        Array.Copy(_init, registers, _init.Length);
        return String.Join(",", Run(registers));
    }

    public string SolvePart2()
    {
        var a = 0L;
        while (true)
        {            
            var output = Run([a, 0, 0]);
            var match = _p.TakeLast(output.Length)
                .Zip(output, (p, ot) => p == ot)                
                .All(b => b);

            if (match && output.Length == _p.Length)
                break;

            a = match ? a << 3 : a + 1;
        }

        return a.ToString();
    }

    private byte[] Run(long[] registers)
    {
        List<byte> output = [];     

        for (int opIdx = 0; opIdx < _p.Length;)        
            opIdx = RunOp(registers, output, opIdx, _p[opIdx + 1], Combo(registers, _p[opIdx + 1]));

        return [.. output];
    }

    private int RunOp(long[] reg, List<byte> output, int opIdx, int literal, long combo) => (Code)_p[opIdx] switch
    {
        Code.ADV => Do(opIdx, reg[A], combo, ref reg[A], Dv),
        Code.BDV => Do(opIdx, reg[A], combo, ref reg[B], Dv),
        Code.CDV => Do(opIdx, reg[A], combo, ref reg[C], Dv),
        
        Code.BST => Do(opIdx, combo, 8, ref reg[B], Mod),
        
        Code.BXL => Do(opIdx, reg[B], literal, ref reg[B], Xor),
        Code.BXC => Do(opIdx, reg[B], reg[C], ref reg[B], Xor),        
        
        Code.JNZ => Jmp(opIdx, reg[A], literal),
        Code.OUT => opIdx + Out(output, combo),

        _ => throw new NotImplementedException()
    };

    #region Operations

    private static int Do(int opIdx, long a, long b, ref long output, Instruction instruction)
    {
        instruction(a, b, ref output);
        return opIdx + 2;
    }

    private static int Out(List<byte> output, long literal)
    {
        output.Add((byte)(literal % 8));
        return 2;
    }

    private static int Jmp(int op, long check, int literal)
        => check == 0 ? op + 2 : literal;

    private static void Mod(long a, long b, ref long output)
        => output = a % b;

    private static void Xor(long a, long b, ref long output)
        => output = a ^ b;

    private static void Dv(long a, long b, ref long output)
        => output = a / (1L << (int)b);

    private static long Combo(long[] r, int n)
        => n >= 4 ? r[n - 4] : n;

    #endregion

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumRegex();
}
