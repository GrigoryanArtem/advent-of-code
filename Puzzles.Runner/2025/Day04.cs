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
        return _data.WithIndex()            
            .Count(d => IsAccessible(_data, d.index, _removed))
            .ToString();
    }

    public string SolvePart2()
    {
        Array.Clear(_removed);


        var total = 0;
        var sum = 0;
        do
        {
            sum = 0;

            foreach (var (item, pos) in _data.WithIndex())
            {
                if (IsAccessible(_data, pos, _removed))
                {
                    _removed[pos] = true;
                    sum++;
                }
            }

            total += sum;
        }
        while (sum > 0);

        return total.ToString();
    }

    private bool IsAccessible(Mat mat, int pos, bool[] removed)
    {
        if (mat[pos] != ROLL || removed[pos])
            return false;

        var count = 0;
        for (int i = 0; i < _moore.Length && count < 4; i++)
        {
            var n = pos + _moore[i];
            if (!removed[n] && mat[n] == ROLL)
                count++;
        }

        return count < 4;
    }

}
