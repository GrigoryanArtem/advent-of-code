using Microsoft.Z3;
using System.Text.RegularExpressions;

namespace Puzzles.Runner._2025;

[Puzzle("Factory", 10, 2025)]
public partial class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private const char ON = '#';

    private record struct Manual(int Size, uint StartState, int[][] Buttons, uint[] EB, int[] Joltage);
    private Manual[] _manuals = [];

    public void Init()
    {
        _manuals = new Manual[input.Lines.Length];

        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            var state = LedRegex().Match(line).Groups["led"].Value;
            var buttons = ButtonRegex().Matches(line).Select(m => m.Groups["button"].Value.Split(',').Select(Int32.Parse).ToArray()).ToArray();
            var size = state.Length;

            _manuals[idx] = new Manual
            (
                Size: size,
                StartState: ES(state),
                Buttons: buttons,
                EB: [..buttons.Select(b => EB(size, b))],
                Joltage: [..JoltageRegex().Match(line).Groups["joltage"].Value.Split(',').Select(Int32.Parse)]
            );
        }
    }

    public string SolvePart1()
    {
        var sum = 0;

        Parallel.For(0, _manuals.Length, i => {
            var steps = Configure(_manuals[i]);
            Interlocked.Add(ref sum, steps);
        });

        return sum.ToString();
    }

    public string SolvePart2()
    {
        var sum = 0;

        Parallel.For(0, _manuals.Length, i => {
            var steps = ConfigureJoltage(_manuals[i]);
            Interlocked.Add(ref sum, steps);
        });

        return sum.ToString();
    }

    private static int ConfigureJoltage(Manual manual)
    {
        var ctx = new Context();
        var opt = ctx.MkOptimize();

        var buttonVars = new IntExpr[manual.Buttons.Length];

        for (var b = 0; b < manual.Buttons.Length; b++)
        {
            buttonVars[b] = ctx.MkIntConst($"b_{b}");
            opt.Add(ctx.MkGe(buttonVars[b], ctx.MkInt(0)));
        }

        for (var j = 0; j < manual.Joltage.Length; j++)
        {
            var terms = new List<ArithExpr>();
            for (var b = 0; b < manual.Buttons.Length; b++)
                if (manual.Buttons[b].Contains(j))
                    terms.Add(buttonVars[b]);

            var se = ctx.MkAdd([.. terms]);
            var te = ctx.MkInt(manual.Joltage[j]);

            opt.Add(ctx.MkEq(se, te));
        }

        opt.MkMinimize(ctx.MkAdd([.. buttonVars.Cast<ArithExpr>()]));

        var status = opt.Check();

        return Enumerable
            .Range(0, manual.Buttons.Length)            
            .Sum(idx => ((IntNum)opt.Model.Evaluate(buttonVars[idx])).Int);
    }

    private static int Configure(Manual manual)
    {        
        PriorityQueue<uint, int> queue = new();
        HashSet<uint> set = [];

        queue.Enqueue(manual.StartState, 0);

        while (queue.TryDequeue(out var state, out var steps))
        {            
            if (set.Contains(state))
                continue;            

            if (state == 0U)
                return steps;

            foreach (var btn in manual.EB)
                queue.Enqueue(state ^ btn, steps + 1);
        }

        return -1;
    }

    // Encode button
    private static uint EB(int size, IEnumerable<int> button)
    {
        var res = 0U;

        foreach (var num in button)
            res |= 1U << (size -1 - num);

        return res;
    }

    // Encode state
    private static uint ES(string state)
    {
        var res = 0U;

        foreach (var ch in state)
            res = (res << 1) | (ch == ON ? 1U : 0U);

        return res;
    }

    [GeneratedRegex(@"\[(?<led>.*?)\]")]
    private static partial Regex LedRegex();

    [GeneratedRegex(@"\((?<button>.*?)\)")]
    private static partial Regex ButtonRegex();

    [GeneratedRegex(@"\{(?<joltage>.*?)\}")]
    private static partial Regex JoltageRegex();
}
