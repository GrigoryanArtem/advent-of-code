using Puzzles.Runner._2019.Common;
using System.Text;

namespace Puzzles.Runner._2019;

[Puzzle("Space Police", 11, 2019)]
public class Day11(ILinesInputReader input) : IPuzzleSolver
{
    private const byte BLACK = 0;
    private const byte WHITE = 1;

    private static readonly Vec2[] _directions = AOC.Directions2DInv;
    private IntCodeMachine _machine = IntCodeMachine.Null;

    public void Init()
        => _machine = IntCodeMachine.FromInput(input, UInt16.MaxValue);

    public string SolvePart1()
        => RunRobot([]).Count.ToString();

    public string SolvePart2()
        => DrawPanel(RunRobot(new() { { Vec2.Zero, WHITE } }));

    #region Private methods

    private Dictionary<Vec2, byte> RunRobot(Dictionary<Vec2, byte> panels)
    {
        _machine.Reset();

        var ddx = 0;
        var pos = Vec2.Zero;

        while (!_machine.Halted)
        {
            _machine.Input(panels.GetOrAdd(pos, BLACK));
            _machine.Run();

            var output = _machine.Output.TakeLast(2);

            panels[pos] = (byte)output.First();
            ddx = AOC.Mod(ddx + (output.Last() == 0 ? -1 : 1), _directions.Length);

            pos += _directions[ddx];
        }

        return panels;
    }

    private static string DrawPanel(Dictionary<Vec2, byte> panels)
    {
        var (minX, maxX) = panels.Keys.MinMax(k => k.X);
        var (minY, maxY) = panels.Keys.MinMax(k => k.Y);

        var sb = new StringBuilder();
        for (int y = minY; y <= maxY; y++)
        {
            sb.AppendLine();

            for (int x = minX; x <= maxX; x++)
                sb.Append(panels.GetValueOrDefault(new(x, y), BLACK) == 1 ? '#' : ' ');
        }

        return sb.ToString();
    }

    #endregion
}
