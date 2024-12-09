using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Puzzles.Runner._2024;

[Puzzle("Disk Fragmenter", 9, 2024)]
public class Day9(IFullInputReader input) : IPuzzleSolver
{
    public record DiskBlock(int Start, int Count, int Value);
    private const int EMPTY = -1;

    private int[] _disk = [];

    public void Init()
    {
        List<int> disk = [];
        List<DiskBlock> blocks = [];

        var free = false;
        var id = 0;
        foreach(var num in input.Text)
        {
            disk.AddRange(Enumerable.Repeat(free ? EMPTY : id++, num - '0'));
            free = !free;
        }

        _disk = [.. disk];
    }

    public string SolvePart1()
    {
        var (left, right) = (0, _disk.Length - 1);

        while (left < right)
        {
            if (_disk[left] != EMPTY)
            {
                left++;
            }
            else if (_disk[right] == EMPTY)
            {
                right--;
            }
            else
            {
                (_disk[left], _disk[right]) = (_disk[right], _disk[left]);

                left++;
                right--;
            }
        }
        
        return _disk.TakeWhile(v => v != EMPTY)
            .WithIndex()
            .Aggregate(0UL, (acc, val) => acc + (ulong)(val.index * val.item)).ToString();
    }
}
