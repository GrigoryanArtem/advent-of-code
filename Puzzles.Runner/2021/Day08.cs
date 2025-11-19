namespace Puzzles.Runner._2021;

[Puzzle("Seven Segment Search", 8, 2021)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private record Example(HashSet<char>[] Patterns, HashSet<char>[] Output);

    private class Resolver
    {
        private HashSet<char>[] _numbers = new HashSet<char>[10];

        public void Init(HashSet<char>[] patterns)
        {
            _numbers[1] = Find(patterns, 2);
            _numbers[4] = Find(patterns, 4);
            _numbers[7] = Find(patterns, 3);
            _numbers[8] = Find(patterns, 7);

            _numbers[2] = Find(patterns, _numbers, 5, 1, 2, 2);
            _numbers[3] = Find(patterns, _numbers, 5, 2, 3, 3);
            _numbers[5] = Find(patterns, _numbers, 5, 1, 3, 2);

            _numbers[0] = Find(patterns, _numbers, 6, 2, 3, 3);
            _numbers[6] = Find(patterns, _numbers, 6, 1, 3, 2);
            _numbers[9] = Find(patterns, _numbers, 6, 2, 4, 3);
        }

        public int Resolve(HashSet<char> probe)
            => _numbers.WithIndex().Single(n => n.item.Count == probe.Count && !n.item.Except(probe).Any()).index;

        public int Resolve(HashSet<char>[] probes)
            => probes.Aggregate(0, (acc, v) => acc * 10 + Resolve(v));

        private static HashSet<char> Find(HashSet<char>[] patterns, int length)
            => patterns.Single(p => p.Count == length);

        private static HashSet<char> Find(HashSet<char>[] patterns, HashSet<char>[] precalc, int length, int one, int four, int seven)
            => patterns.Single(p => p.Count == length &&
                precalc[1].Intersect(p).Count() == one &&
                precalc[4].Intersect(p).Count() == four &&
                precalc[7].Intersect(p).Count() == seven);
    }

    private Example[] _examples = [];

    public void Init()
    {
        _examples = [.. input.Lines.Select(line =>
        {
            var tokens = line.Split("|");
            var patterns = tokens[0].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var output = tokens[1].Split(' ', StringSplitOptions.RemoveEmptyEntries);

            return new Example
            (
                [..patterns.Select(str => str.ToHashSet())],
                [..output.Select(str => str.ToHashSet())]
            );
        })];
    }

    public string SolvePart1()
        => _examples.Sum(e => e.Output.Count(x => GetSimpleDigit(x) is not null)).ToString();

    public string SolvePart2()
    {
        var resolver = new Resolver();
        return _examples.Sum(ex =>
        {
            resolver.Init(ex.Patterns);
            return resolver.Resolve(ex.Output);
        }).ToString();
    }

    public static int? GetSimpleDigit(HashSet<char> value) => value.Count switch
    {
        2 => 1,
        4 => 4,
        3 => 7,
        7 => 8,

        _ => null
    };
}
