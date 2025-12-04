namespace Puzzles.Runner._2025;

using Mat = Mat2<char>;

[Puzzle("Printing Department", 4, 2025)]
public partial class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private const char EMPTY = '.';
    private const char ROLL = '@';

    private Mat _data = Mat.Null;
    private int[] _moore = [];
    private bool[] _removed = [];

    public void Init()
    {
        _data = Mat.WithBorders([.. input.Lines.SelectMany(line => line.ToCharArray())], input.Lines[0].Length, EMPTY);
        _removed = _data.CreateBuffer<bool>();

        _moore =
        [
            1, -1,
            -_data.Columns, _data.Columns,
            -_data.Columns - 1, -_data.Columns + 1,
            _data.Columns - 1,  _data.Columns + 1,
        ];
    }

    public string SolvePart1()
    {
        Array.Clear(_removed);
        return Enumerable.Range(0, _data.Data.Length) 
            .Where(idx => _data[idx] == ROLL)
            .Count(IsAccessible)
            .ToString();
    }

    public string SolvePart2()
    {
        Array.Clear(_removed);

        var rolls = new Queue<int>(_data.WithIndex()            
            .Where(d => d.item == ROLL)
            .Select(d => d.index));
        var next = new Queue<int>();

        var total = 0;
        var sum = 0;
        do
        {
            sum = 0;

            while(rolls.TryDequeue(out var pos))
            {                
                if (IsAccessible(pos))
                {
                    _removed[pos] = true;
                    sum++;                    
                }
                else
                {
                    next.Enqueue(pos);
                }
            }

            (rolls, next) = (next, rolls);
            total += sum;
        }
        while (sum > 0);

        return total.ToString();
    }

    private bool IsAccessible(int pos)
    {
        var count = 0;
        for (int i = 0; i < _moore.Length && count < 4; i++)
        {
            var n = pos + _moore[i];
            if (!_removed[n] & _data[n] == ROLL)
                count++;
        }

        return count < 4;
    }
}
