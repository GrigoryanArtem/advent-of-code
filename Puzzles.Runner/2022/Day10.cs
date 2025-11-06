using System.Text;

namespace Puzzles.Runner._2022;

[Puzzle("Cathode-Ray Tube", 10, 2022)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private enum OpCode
    {
        Noop,
        Addx
    };

    private record Instruction(OpCode OpCode, int Value);
    private Instruction[] _program = [];

    public void Init()
    {
        _program = [..input.Lines.Select(line =>
        {
            var tokens = line.Split(' ', 2);
            return new Instruction
            (
                OpCode: Enum.Parse<OpCode>(tokens[0], true), 
                Value: tokens.Length > 1 ? Convert.ToInt32(tokens[1]) : 0);
        })];
    }

    public string SolvePart1()
    {
        var samples = new HashSet<int> { 20, 60, 100, 140, 180, 220 };
        return Run(_program)
            .WithIndex()
            .Where(d => samples.Contains(d.index + 1))
            .Sum(d => (d.index + 1) * d.item)
            .ToString();
    }

    public string SolvePart2()
    {
        var sb = new StringBuilder();

        foreach (var (x, idx) in Run(_program).Select((d, i) => (d, i % 40)))
        {
            if(idx == 0)
                sb.AppendLine();

            sb.Append(Math.Abs(idx - x) <= 1 ? '#' : ' ');
        }

        return sb.ToString();
    }

    private static IEnumerable<int> Run(Instruction[] program) 
    {
        int x = 1;
        foreach (var instr in program)
        {
            if(instr.OpCode == OpCode.Noop)
                yield return x;

            if(instr.OpCode == OpCode.Addx)
            {
                yield return x;
                yield return x;
                x += instr.Value;
            }
        }
    }
}
