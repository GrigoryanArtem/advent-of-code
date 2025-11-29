using System.Runtime.CompilerServices;

namespace Puzzles.Runner._2020;

[Puzzle("Handheld Halting", 8, 2020)]
public partial class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private enum Op { acc, jmp, nop }
    private record struct Instruction(Op Op, int Argument);


    private Instruction[] _program = [];

    public void Init()
        => _program = [.. input.GetTokens(" ", x => x)
                .Select(t => new Instruction
                (
                    Op: Enum.Parse<Op>(t[0]),
                    Argument: Convert.ToInt32(t[1])
                ))];

    public string SolvePart1()
    {
        var buffer = new bool[_program.Length];
        TryRun(_program, out var acc, buffer);
        return acc.ToString();
    }

    public string SolvePart2()
    {
        var cycle = FindCycle(_program);

        var corrected = new Instruction[_program.Length];
        Array.Copy(_program, corrected, corrected.Length);

        var buffer = new bool[_program.Length];
        foreach (var probe in cycle.Where(idx => _program[idx].Op != Op.acc))
        {
            corrected[probe] = _program[probe].Op == Op.jmp
                ? _program[probe] with { Op = Op.nop }
                : _program[probe] with { Op = Op.jmp };

            if (TryRun(corrected, out var acc, buffer))
                return acc.ToString();

            corrected[probe] = _program[probe];
        }

        return "NOT CALCULATED";
    }

    private static bool TryRun(Instruction[] program, out int acc, bool[] buffer)
    {
        Array.Clear(buffer);
        int cur = 0;
        acc = 0;

        while (cur < program.Length)
        {
            if (buffer[cur])
                return false;

            ref var instr = ref program[cur];
            buffer[cur] = true;

            (acc, cur) = instr.Op switch
            {
                Op.acc => (acc + instr.Argument, cur + 1),
                Op.jmp => (acc, cur + instr.Argument),
                _ => (acc, cur + 1)
            };
        }

        return true;
    }

    private static int[] FindCycle(Instruction[] program)
    {        
        int slow = 0;
        int fast = 0;

        do
        {            
            slow = Step(program, slow);

            fast = Step(program, fast);
            fast = Step(program, fast);
        } while (slow != fast);


        var cycle = new List<int>();
        do
        {
            cycle.Add(slow);
            slow = Step(program, slow);
        } while (slow != fast);


        return [..cycle];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static int Step(Instruction[] program, int iidx)
    {
        ref var instr = ref program[iidx];
        return iidx + (instr.Op == Op.jmp ? instr.Argument : 1);
    }
}
