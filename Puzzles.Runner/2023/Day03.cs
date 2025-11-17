namespace Puzzles.Runner._2023;

using Map = Mat2<char>;

[Puzzle("Gear Ratios", 3, 2023)]
public partial class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private Map _engine = Map.Null;
    private int[] _dirs = [];

    public void Init()
    {
        _engine = Map.WithBorders
        (
            [.. input.Lines.SelectMany(s => s.AsEnumerable())],
            input.Lines.First().Length,
            '.'
        );

        _dirs =
        [
            1, -1,
            -_engine.Columns, _engine.Columns,
            -_engine.Columns - 1, -_engine.Columns + 1, 
            _engine.Columns - 1,  _engine.Columns + 1,
        ];
    }

    public string SolvePart1()
        =>  ExtractNumbers(_engine)
            .Sum(d => d.withSymbol ? d.number : 0)
            .ToString();    

    public string SolvePart2()
    {
        var gears = _engine.Data.WithIndex()
            .Where(d => d.item == '*')
            .Select(d => d.index)
            .ToArray();

        var numbers = ExtractNumbers(_engine).ToArray();
        return gears.Sum(g =>
        {
            var targets = numbers.Where(n => n.geers.Contains(g)).ToArray();
            return targets.Length == 2 ? targets[0].number * targets[1].number : 0;
        }).ToString();
    }

    private IEnumerable<(int number, bool withSymbol, HashSet<int> geers)> ExtractNumbers(Map engine)
    {
        for (int r = 0; r < engine.Rows; r++)
        {
            for (int c = 0; c < _engine.Columns; c++)
            {
                var loc = _engine.D2toD1(c, r);
                if (!Char.IsDigit(_engine[loc]))
                    continue;

                var hasSymbol = false;
                var end = loc;
                HashSet<int> geers = [];

                while (Char.IsDigit(_engine[end]))
                {
                    var symbols = _dirs.Select(d => end + d)
                        .Where(d => !Char.IsDigit(_engine[d]) && _engine[d] != '.')
                        .ToArray();

                    hasSymbol |= hasSymbol || symbols.Length != 0;
                    geers.UnionWith(symbols.Where(d => _engine[d] == '*'));

                    end++;
                    c++;
                }

                yield return (Int32.Parse(_engine.Data.AsSpan()[loc..end]), hasSymbol, geers);
            }
        }
    }
}
