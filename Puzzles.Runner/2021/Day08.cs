using System.Numerics;

namespace Puzzles.Runner._2021;

[Puzzle("Seven Segment Search", 8, 2021)]
public class Day08(ILinesInputReader input) : IPuzzleSolver
{    
    private record Example(uint[] Patterns, uint[] Output);
    private class Resolver
    {
        private readonly uint[] _numbers = new uint[10];        

        public int Resolve(uint[] patterns, uint[] probes)
        {
            Init(patterns);
            return probes.Aggregate(0, (acc, v) => acc * 10 + Resolve(v));
        }

        private void Init(uint[] patterns)
        {
            _numbers[1] = Find(patterns, 2);
            _numbers[7] = Find(patterns, 3);
            _numbers[4] = Find(patterns, 4);
            _numbers[8] = Find(patterns, 7);

            _numbers[3] = Find(patterns, 5, 7, 3);
            _numbers[9] = Find(patterns, 6, 4, 4);
            _numbers[6] = Find(patterns, 6, 7, 2);
            _numbers[5] = Find(patterns, 5, 6, 5);
            _numbers[0] = Find(patterns, 6, 5, 4);
            _numbers[2] = Find(patterns, 5, 4, 2);
        }

        private int Resolve(uint probe)
            => Array.IndexOf(_numbers, probe);

        private static uint Find(uint[] patterns, int length)
            => patterns.First(p => BitOperations.PopCount(p) == length);

        private uint Find(uint[] patterns, int length, int @ref, int count)
            => patterns.First(p => 
                BitOperations.PopCount(p) == length &&
                BitOperations.PopCount(_numbers[@ref] & p) == count);
    }

    private Example[] _examples = [];
    private static readonly ThreadLocal<Resolver> _resolver = new(() => new Resolver(), trackAllValues: false);

    public void Init()
    {
        _examples = [.. input.Lines.Select(line =>
        {
            var tokens = line.Split("|");
            return new Example
            (
                [..tokens[0].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ToMask)],
                [..tokens[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(ToMask)]
            );
        })];
    }

    public string SolvePart1()
        => _examples.Sum(e => e.Output.Count(IsSimpleDigit)).ToString();

    public string SolvePart2() 
        => _examples.AsParallel()            
            .Sum(ex => _resolver.Value!.Resolve(ex.Patterns, ex.Output))
            .ToString();

    private static bool IsSimpleDigit(uint value) => BitOperations.PopCount(value) switch
    {
        2 or 4 or 3 or 7 => true,
        _ => false
    };

    private static uint ToMask(string str)
    {
        var result = 0u;

        foreach (char c in str)
            result |= 1u << (c - 'a');

        return result;
    }
}
