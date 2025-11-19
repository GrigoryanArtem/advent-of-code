namespace Puzzles.Runner._2021;

[Puzzle("Seven Segment Search", 8, 2021)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{    
    private record Example(string[] Patterns, string[] Output);

    private class Resolver
    {
        private readonly string[] _numbers = new string[10];
        public int Resolve(string[] patterns, string[] probes)
        {
            Init(patterns);
            return probes.Aggregate(0, (acc, v) => acc * 10 + Resolve(v));
        }

        private void Init(string[] patterns)
        {
            _numbers[1] = Find(patterns, 2);
            _numbers[7] = Find(patterns, 3);
            _numbers[4] = Find(patterns, 4);
            _numbers[8] = Find(patterns, 7);

            _numbers[2] = Find(patterns, _numbers, 5, 1, 2);
            _numbers[3] = Find(patterns, _numbers, 5, 2, 3);
            _numbers[5] = Find(patterns, _numbers, 5, 1, 3);

            _numbers[0] = Find(patterns, _numbers, 6, 2, 3);
            _numbers[6] = Find(patterns, _numbers, 6, 1, 3);
            _numbers[9] = Find(patterns, _numbers, 6, 2, 4);
        }

        private int Resolve(string probe)
            => Array.IndexOf(_numbers, probe);

        private static string Find(string[] patterns, int length)
            => patterns.First(p => p.Length == length);

        private static string Find(string[] patterns, string[] precalc, int length, int one, int four)
            => patterns.First(p => p.Length == length &&
                IntersectcCount(precalc[1], p) == one &&
                IntersectcCount(precalc[4], p) == four);

        private static int IntersectcCount(string a, string b)
            => a.Count(b.Contains);
    }

    private Example[] _examples = [];

    public void Init()
    {
        _examples = [.. input.Lines.Select(line =>
        {
            var tokens = line.Split("|");
            return new Example
            (
                [..tokens[0].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => new string([..str.OrderBy(x => x)]))],

                [..tokens[1].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(str => new string([..str.OrderBy(x => x)]))]
            );
        })];
    }

    public string SolvePart1()
        => _examples.Sum(e => e.Output.Count(IsSimpleDigit)).ToString();

    public string SolvePart2()
    {
        var resolver = new Resolver();
        return _examples.Sum(ex => resolver.Resolve(ex.Patterns, ex.Output)).ToString();
    }

    public static bool IsSimpleDigit(string value) => value.Length switch
    {
        2 or 4 or 3 or 7 => true,
        _ => false
    };
}
