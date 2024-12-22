using Puzzles.Base;

namespace Puzzles.Visuals;

internal class Program
{
    static void Main(string[] args)
    {
        var reader = new LinesInputReader("input.in");
        var day20 = new Day20(reader);

        day20.Run();
        day20.Save();

    }
}
