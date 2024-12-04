using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Ceres Search", 4, 2024)]
public partial class Day3(IFullInputReader input) : IPuzzleSolver
{
    #region Constants

    private record Point2D(int x, int y);

    private readonly char[] XMAS = ['X', 'M', 'A', 'S'];
    private readonly char[] MAS = ['M', 'A', 'S'];

    private readonly Point2D[][] XMAS_CHECK =
    [
        [new (-0, +0), new (-1, +1), new (-2, +2), new (-3, +3)], // TR
        [new (-0, +0), new (+1, +0), new (+2, +0), new (+3, +0)], //  R
        [new (-0, +0), new (+1, +1), new (+2, +2), new (+3, +3)], // BR
        [new (-0, +0), new (-0, +1), new (-0, +2), new (-0, +3)], //  B
    ];

    private readonly Point2D[][] MAS_CHEK_TOP =
    [
        [new (-0, +0), new (+1, +1), new (+2, +2)],
    ];

    private readonly Point2D[][] MAS_CHECK_BOTTOM =
    [
        [new (+0, +2), new (+1, +1), new (+2, +0)],
    ];

    #endregion

    private readonly char[] _text = [.. NewLineRegex().Replace(input.Text, string.Empty)];
    private Point2D _size;

    public void Init()
    {
        var sizeX = input.Text.IndexOf('\n') - 1;
        _size = new(sizeX, _text.Length / sizeX);
    }

    public string SolvePart1()
        => _text.Select((_, i) => i).Sum(idx => Check(XMAS_CHECK, idx / _size.x, idx % _size.x, XMAS)).ToString();

    public string SolvePart2()
        => _text.Select((_, i) => i).Count(idx => Check(MAS_CHEK_TOP, idx / _size.x, idx % _size.x, MAS) > 0 && 
            Check(MAS_CHECK_BOTTOM, idx / _size.x, idx % _size.x, MAS) > 0).ToString();

    #region Additional 

    private int Check(Point2D[][] source, int x, int y, char[] template)
    {
        var straight = source.Count(check => check
                .Select((charIdx, templateIdx) => (x: charIdx.x + x, y: charIdx.y + y, templateIdx))
                .All(d => TryGetSymbol(d.x, d.y, out var symbol) && symbol == template[d.templateIdx]));

        var reverse = source.Count(check => check
                .Select((charIdx, templateIdx) => (x: charIdx.x + x, y: charIdx.y + y, templateIdx: template.Length - templateIdx - 1))
                .All(d => TryGetSymbol(d.x, d.y, out var symbol) && symbol == template[d.templateIdx]));

        return straight + reverse;
    }

    private bool TryGetSymbol(int x, int y, out char symbol)
    {
        symbol = default;
        var rowCheck = y >= 0 && y < _size.y;
        var columnCheck = x >= 0 && x < _size.x;

        if (!rowCheck || !columnCheck)
            return false;

        symbol = _text[y * _size.x + x];
        return true;
    }

    [GeneratedRegex("\n|\r")]
    private static partial Regex NewLineRegex();

    #endregion
}
