using Puzzles.Base;
using Puzzles.Visuals._2024;

namespace Puzzles.Visuals;

internal class Program
{
    static void Main(string[] args)
    {
        var reader = new LinesInputReader("input.in");
        var day20 = new Day15(reader);

        day20.Run();
        // day20.Save();

    }
}
