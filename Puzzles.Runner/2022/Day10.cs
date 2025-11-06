namespace Puzzles.Runner._2022;

[Puzzle("Rope Bridge", 10, 2022)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private record Instruction(string Opcode, int Value);

    private Instruction[] _program = [];

    public void Init()
    {
        _program = [..input.Lines.Select(line =>
        {
            var tokens = line.Split(' ', 2);
            return new Instruction(tokens[0], tokens.Length > 1 ? Convert.ToInt32(tokens[1]) : 0);
        })];
    }

    public string SolvePart1()
    {
        var e = Run(_program).GetEnumerator();

        Loop(e, 20);
        long sum = 20 * e.Current;
        
        for(int i = 1; i <= 5; i++)
        {
            Loop(e, 40);
            sum += (20 + i * 40) * e.Current;
        }

        return sum.ToString();
    }

    private void Loop(IEnumerator<int> enumerator, int cycles)
    {
        for (int i = 0; i < cycles; i++)
            enumerator.MoveNext();
    }

    private IEnumerable<int> Run(Instruction[] program) 
    {
        int x = 1;

        foreach (var instr in program)
        {
            if(instr.Opcode == "noop")
                yield return x;

            if(instr.Opcode == "addx")
            {
                yield return x;
                yield return x;
                x += instr.Value;
            }
        }

        while (true)
            yield return x;
    }
}
