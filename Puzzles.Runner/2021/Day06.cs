namespace Puzzles.Runner._2021;

[Puzzle("Lanternfish", 6, 2021)]
public class Day06(IFullInputReader input) : IPuzzleSolver
{
    private const int COUNT = 9;
    private ulong[] _input = [];

    public void Init()
    {
        _input = new ulong[COUNT];
        foreach (var num in input.Text.Split(',').Select(UInt64.Parse))
            _input[num]++;
    }

    public string SolvePart1()
    {
        var buffer = new ulong[COUNT];
        Array.Copy(_input, buffer, buffer.Length);

        return CalculateFishCount(buffer, 80).ToString();
    }

    public string SolvePart2()
    {
        var buffer = new ulong[COUNT];
        Array.Copy(_input, buffer, buffer.Length);

        return CalculateFishCount(buffer, 256).ToString();
    }

    public static ulong CalculateFishCount(ulong[] buffer, int days)
    {
        for (int d = 0; d < days; d++)
        {
            var newFish = buffer[0];
            for (int i = 1; i < buffer.Length; i++)
                buffer[i - 1] = buffer[i];

            buffer[8] = newFish;
            buffer[6] += newFish;
        }

        return buffer.UInt64Sum(x => x);
    }
}
