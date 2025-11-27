using System.Numerics;

namespace Puzzles.Runner._2020;

[Puzzle("Custom Customs", 6, 2020)]
public class Day06(IFullInputReader input) : IPuzzleSolver
{
    private string[][] _groups = [];

    public void Init()
        => _groups = [.. input.Text.Split("\r\n\r\n").Select(g => g.Split("\r\n").ToArray())];    

    public string SolvePart1()
        => _groups
            .AsParallel()
            .Select(p => p.Aggregate(0U, (acc, g) => acc | S2A(g)))
            .Sum(BitOperations.PopCount)
            .ToString();

    public string SolvePart2()
        => _groups
            .AsParallel()
            .Select(g => g.Aggregate(UInt32.MaxValue, (acc, g) => acc & S2A(g)))
            .Sum(BitOperations.PopCount)
            .ToString();

    private static uint S2A(string str)
    {
        var result = 0U;
        foreach (var ch in str)
            result |= (1U << (ch - 'a'));

        return result;
    }
}
