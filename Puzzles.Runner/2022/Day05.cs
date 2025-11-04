using System.Text.RegularExpressions;

using Stacks = System.Collections.Generic.Stack<char>[];

namespace Puzzles.Runner._2022;

[Puzzle("Supply Stacks", 5, 2022)]
public partial class Day05(ILinesInputReader input) : IPuzzleSolver
{    
    private record Move(int Count, int From, int To);

    private Move[] _moves = [];
    private Stacks _stacks = [];
    private readonly Stack<char> _buffer = [];

    public void Init()
    {
        var splitIndex = input.Lines.TakeWhile(s => !String.IsNullOrEmpty(s)).Count();
        var idLine = input.Lines[splitIndex - 1];

        _stacks = [..idLine.WithIndex()
            .Where(ch => ch.item != ' ')
            .Select(d => new Stack<char>(input.Lines
                .Take(splitIndex - 1)
                .Select(line => line[d.index])
                .Where(ch => ch != ' ')))];

        _moves = [.. input.Lines.Skip(splitIndex + 1)
            .Select(line => 
            {
                var m = MoveRegex().Match(line);
                return new Move
                (
                    Convert.ToInt32(m.Groups["count"].Value),
                    Convert.ToInt32(m.Groups["from"].Value),
                    Convert.ToInt32(m.Groups["to"].Value)
                );
            })];
    }

    public string SolvePart1()
    {
        var stacks = StacksCopy();
        _moves.ForEach(m => ExecuteMove9000(stacks, m));                

        return new string([.. stacks.Select(s => s.TryPeek(out var ch) ? ch : ' ')]);
    }

    public string SolvePart2()
    {
        var stacks = StacksCopy();
        _moves.ForEach(m => ExecuteMove9001(stacks, m));

        return new string([.. stacks.Select(s => s.TryPeek(out var ch) ? ch : ' ')]);
    }

    private static void ExecuteMove9000(Stacks stacks, Move move)
    {
        for (int i = 0; i < move.Count; i++)
            stacks[move.To - 1].Push(stacks[move.From - 1].Pop());
    }

    private void ExecuteMove9001(Stacks stacks, Move move)
    {
        _buffer.Clear();

        for (int i = 0; i < move.Count; i++)
            _buffer.Push(stacks[move.From - 1].Pop());

        while (_buffer.TryPop(out var ch))
            stacks[move.To - 1].Push(ch);
    }

    private Stacks StacksCopy()
        => [.. _stacks.Select(s => new Stack<char>(s))];

    [GeneratedRegex(@"move\s+(?<count>\d+)\s+from\s+(?<from>\d+)\s+to\s+(?<to>\d+)")]
    private static partial Regex MoveRegex();
}
