using Puzzles.Runner.Base;

namespace Puzzles.Runner._2017;

[Puzzle("Memory Reallocation", 6, 2017)]
public class Day06(IFullInputReader input) : IPuzzleSolver
{
    private static readonly UInt128 UINT128_ONES =
        ((UInt128)0x0101010101010101UL << 64) | 0x0101010101010101UL;

    private int[] _blocks = [];
    private int[] _buffer = [];

    public void Init()
    {        
        _blocks = [.. input.Text.Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Convert.ToInt32(s))];
        _buffer = new int[_blocks.Length];
    }

    public string SolvePart1()
    {
        Array.Copy(_blocks, _buffer, _blocks.Length);

        var (cyclePosition, _) = FindCycle(_buffer);

        return cyclePosition.ToString();
    }

    public string SolvePart2()
    {
        Array.Copy(_blocks, _buffer, _blocks.Length);

        var (_, cycleLength) = FindCycle(_buffer);

        return cycleLength.ToString();
    }

    private static (int cyclePosition, int cycleLength) FindCycle(int[] blocks)
    {
        var state = Arr2U128(blocks);

        Dictionary<UInt128, int> dict = new()
        {
            [state] = 0
        };

        for (var cycle = 1; ; cycle++)
        {
            state = Redistribute(state);

            if (dict.TryGetValue(state, out var pos))
                return (cycle, cycle - pos);

            dict.Add(state, cycle);
        }
    }

    private static UInt128 Redistribute(UInt128 state)
    {
        var maxIndex = 0;
        var maxValue = 0;

        for (var i = 0; i < 16; i++)
        {
            var value = (int)((state >> (i * 8)) & 0xFF);

            if (value > maxValue)
            {
                maxValue = value;
                maxIndex = i;
            }
        }

        state &= ~((UInt128)0xFF << (maxIndex * 8));

        var rounds = maxValue / 16;
        var remaining = maxValue % 16;

        state += UINT128_ONES * (UInt128)(uint)rounds;

        if (remaining > 0)
        {
            var mask = UINT128_ONES >> ((16 - remaining) * 8);
            var shift = ((maxIndex + 1) & 15) * 8;
            
            mask = (mask << shift) | (mask >> (128 - shift));

            state += mask;
        }

        return state;
    }


    private static UInt128 Arr2U128(int[] blocks)
    {
        UInt128 state = 0;

        for (var i = 0; i < blocks.Length; i++)
            state |= (UInt128)(uint)blocks[i] << (i * 8);

        return state;
    }
}
