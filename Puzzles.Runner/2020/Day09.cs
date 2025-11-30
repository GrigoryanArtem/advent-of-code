namespace Puzzles.Runner._2020;

[Puzzle("Encoding Error", 9, 2020)]
public partial class Day09(ILinesInputReader input) : IPuzzleSolver
{
    private const int PREAMBLE = 25;
    private ulong[] _input = [];

    public void Init()
        => _input = [.. input.Lines.Select(UInt64.Parse)];    

    public string SolvePart1()
        => FirstInvalid(_input, PREAMBLE).ToString();

    public string SolvePart2()
    {
        var span = _input.AsSpan();
        var target = FirstInvalid(span, PREAMBLE);

        var sum = 0UL;
        var min = 0UL;
        var max = 0UL;

        for (int right = 0, left = 0; !(min > 0UL || max > 0UL) && right < span.Length; right++)
        {
            sum += span[right];

            while(sum > target)            
                sum -= span[left++];

            if(sum == target) 
            { 
                (min, max) = span[left..(right + 1)].MinMax(x => x);
                break;
            }
        }

        return (min + max).ToString();
    }

    private static ulong FirstInvalid(Span<ulong> numbers, int preamble)
    {
        var freqs = new Dictionary<ulong, int>();
        foreach(var pnum in numbers[..preamble])
            if(!freqs.TryAdd(pnum, 1))
                freqs[pnum]++;

        for (int i = preamble; i < numbers.Length; i++)
        {
            var isValid = false;

            foreach(var (num, freq) in freqs)
            {
                var target = numbers[i] - num;
                isValid = target == num ? freq > 1 : freqs.ContainsKey(target);

                if (isValid)
                    break;
            }

            if (!isValid)
                return numbers[i];            

            var old = numbers[i - preamble];

            freqs[old]--;
            if (freqs[old] == 0)
                freqs.Remove(old);

            if (!freqs.TryAdd(numbers[i], 1))
                freqs[numbers[i]]++;
        }

        return 0UL;
    }
}
