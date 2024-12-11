namespace Puzzles.Runner._2024;

[Puzzle("Disk Fragmenter", 9, 2024)]
public class Day09(IFullInputReader input) : IPuzzleSolver
{
    public record DiskBlock(int Count, int Value);
    private const int EMPTY = -1;

    private DiskBlock[] _blocks = [];
    private DiskBlock[] _buffer = [];

    public void Init()
    {
        List<DiskBlock> blocks = [];

        var free = false;
        var id = 0;
        var index = 0;
        foreach(var digit in input.Text)
        {
            var num = digit - '0';
            blocks.Add(new(num, free ? EMPTY : id++));
            index += num;
            free = !free;
        }

        _blocks = [.. blocks];
        _buffer = new DiskBlock[_blocks.Length];
    }

    public string SolvePart1()
    {
        Array.Copy(_blocks, _buffer, _blocks.Length);

        List<DiskBlock> defragmented = [];
        var (left, right) = (0, _buffer.Length - 1);

        while (left <= right)
        {
            if (_buffer[left].Value != EMPTY)
            {
                defragmented.Add(_buffer[left]);
                left++;
            }
            else if (_buffer[right].Value == EMPTY)
            {
                right--;
            }
            else
            {
                var count = Math.Min(_buffer[left].Count, _buffer[right].Count);
                defragmented.Add(new(count, _buffer[right].Value));

                if (_buffer[left].Count == count)
                {
                    left++;
                }
                else
                {
                    _buffer[left] = new(_buffer[left].Count - count, _buffer[left].Value);
                }

                if (_buffer[right].Count == count)
                {
                    right--;
                }
                else
                {
                    _buffer[right] = new(_buffer[right].Count - count, _buffer[right].Value);
                }
            }
        }

        return CheckSum(defragmented).ToString();
    }

    public string SolvePart2()
    {
        var buffer = new List<DiskBlock>(_blocks);

        for(int tail = buffer.Count - 1; tail >= 0; tail--)
        {
            if (buffer[tail].Value == EMPTY)
                continue;

            for(int head = 0; head < tail; head++)
            {
                if (buffer[head].Value == EMPTY && buffer[head].Count >= buffer[tail].Count)
                {
                    var count = buffer[tail].Count;
                    var diff = buffer[head].Count - count;

                    buffer[head] = new(count, buffer[tail].Value);
                    buffer[tail] = new(count, EMPTY);

                    if (diff > 0)
                    {
                        buffer.Insert(head + 1, new(diff, EMPTY));
                        tail++;
                    }

                    break;
                }
            }
        }

        return CheckSum(buffer).ToString();
    }

    private static ulong CheckSum(IEnumerable<DiskBlock> disk)
        => disk.Aggregate((sum: 0UL, index: 0), (acc, b) =>
            (acc.sum + CheckSum(b, acc.index), acc.index + b.Count)).sum;

    private static ulong CheckSum(DiskBlock block, int index)
        => block.Value == EMPTY ? 0UL : (ulong)((index + (index + block.Count - 1)) * block.Count) / 2UL * (ulong)block.Value;
}
