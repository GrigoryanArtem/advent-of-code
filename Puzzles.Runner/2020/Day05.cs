namespace Puzzles.Runner._2020;

[Puzzle("Binary Boarding", 5, 2020)]
public partial class Day05(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => input.Lines.AsParallel().Select(GetSeatId).Max().ToString();

    public string SolvePart2()
    {
        int min = Int32.MaxValue;
        int max = Int32.MinValue;
        int sum = 0;

        foreach(var id in input.Lines.Select(GetSeatId))
        {
            min = Math.Min(min, id);
            max = Math.Max(max, id);            
            sum += id;
        }
        
        var aps = AOC.ArithmeticProgressionSum(min, max, max - min + 1);
        return (aps - sum).ToString();
    }        

    private static int GetSeatId(string bsp)
        => (Parse(bsp[..7], 'B') << 3) + Parse(bsp[7..], 'R');

    private static int Parse(string str, char val)
        => str.Aggregate(0, (acc, v) => (acc << 1) | (v == val ? 1 : 0));
}
