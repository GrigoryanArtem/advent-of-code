namespace Puzzles.Runner._2025;

using Mat64 = Mat2<ulong>;
using MatS = Mat2<string>;

[Puzzle("Trash Compactor", 6, 2025)]
public class Day06(ILinesInputReader input) : IPuzzleSolver
{
    private record struct Operation(char Op, int Len);
    
    private Operation[] _operations = [];

    public void Init()
        => _operations = GetOperations(input.Lines.Last());
    

    public string SolvePart1()
    {
        var mat = new Mat64([.. input.Lines[..^1].SelectMany(line => line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(UInt32.Parse))], _operations.Length);

        return _operations.WithIndex()
            .UInt64Sum(d => d.item.Op == '*'
                ? mat.Column(d.index).UInt64Mul(a => a)
                : mat.Column(d.index).UInt64Sum(a => a))
            .ToString();
    }

    public string SolvePart2()
    {        
        var mat = new MatS([..input.Lines[..^1].SelectMany(s => 
            S2OpS(s, _operations))], _operations.Length);

        return _operations.WithIndex()
            .UInt64Sum(d => d.item.Op == '*' 
                ? ToRtL(mat, d.index).UInt64Mul(x => x)
                : ToRtL(mat, d.index).UInt64Sum(x => x))
            .ToString();
    }

    private static IEnumerable<ulong> ToRtL(MatS mat, int idx)
    {
        var length = mat.Column(idx).First().Length;

        for (int i = 0; i < length; i++)
        {
            var num = 0UL;
            foreach (var ss in mat.Column(idx))
            {
                if (Char.IsWhiteSpace(ss[i]))
                    continue;

                num = num * 10 + ss[i] - '0';
            }

            yield return num;
        }

    }

    private static Operation[] GetOperations(string s)
    {
        List<Operation> operations = [];

        char? op = null;
        var count = 0;

        for (int i = 0; i < s.Length; i++)
        {
            if (Char.IsWhiteSpace(s[i]))
            {
                count++;
            }
            else
            {
                if (op.HasValue)
                    operations.Add(new(op.Value, count - 1));

                op = s[i];
                count = 1;
            }
        }

        if (op.HasValue)
            operations.Add(new(op.Value, count));

        return [.. operations];
    }

    private static string[] S2OpS(string s, Operation[] operations)
    {
        var cur = 0;
        List<string> str = [];

        foreach (var op in operations)
        {
            str.Add(s[cur..(cur + op.Len)]);
            cur = cur + op.Len + 1;
        }

        return [.. str];
    }
}
