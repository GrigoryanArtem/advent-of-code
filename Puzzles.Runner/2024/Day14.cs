using Puzzles.Base.Entites;
using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Restroom Redoubt", 14, 2024)]
public partial class Day14(IFullInputReader input) : IPuzzleSolver
{
    private record Robot(Vec2 Pos, Vec2 Vel);
    private record Block(Vec2 Start, Vec2 End);

    private Robot[] _robots = [];
    private Vec2[] _predictionBuffer = [];

    public void Init()
    {
        _robots = RobotRegex().Matches(input.Text).Select(m => 
            new Robot(
                Pos: new(M2I32(m, "px"), M2I32(m, "py")),
                Vel: new(M2I32(m, "vx"), M2I32(m, "vy"))
            )).ToArray();

        _predictionBuffer = _robots.Select(r => r.Pos).ToArray();        
    }

    public string SolvePart1()
    {
        var size = new Vec2(101, 103);
        var blocks = new Block[] 
        {
            new(new(0, 0), new(size.X / 2, size.Y / 2)),
            new(new(size.X / 2 + 1, 0), new(size.X, size.Y / 2)),
            new(new(0, size.Y / 2 + 1), new(size.X / 2, size.Y)),
            new(new(size.X / 2 + 1, size.Y / 2 + 1), new(size.X, size.Y)),
        };

        Predict(size, 100, _predictionBuffer);

        return blocks.Select(b => _predictionBuffer.Count(p => 
                p.X >= b.Start.X && p.X < b.End.X &&
                p.Y >= b.Start.Y && p.Y < b.End.Y
            )).Aggregate(1, (acc, v) => acc * v).ToString();
    }

    public string SolvePart2()
    {        
        var size = new Vec2(101, 103);
        var set = new HashSet<Vec2>();

        int iteration = 0;
        for (bool valid = false; !valid;)
        {
            set.Clear();
            Predict(size, ++iteration, _predictionBuffer);
            set.UnionWith(_predictionBuffer);

            valid = set.Count == _predictionBuffer.Length;                
        }

        return iteration.ToString();
    }

    #region Private methods

    private void Predict(Vec2 size, int iterations, Vec2[] buffer)
    {
        for(int i = 0; i < _robots.Length; i++)
        {
            buffer[i].X = AOC.Mod(_robots[i].Pos.X + _robots[i].Vel.X * iterations, size.X);
            buffer[i].Y = AOC.Mod(_robots[i].Pos.Y + _robots[i].Vel.Y * iterations, size.Y);
        }
    }

    private static int M2I32(Match match, string group)
        => Convert.ToInt32(match.Groups[group].Value);

    [GeneratedRegex(@"p=(?<px>-?\d+),(?<py>-?\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)", RegexOptions.Compiled)]
    private static partial Regex RobotRegex();

    #endregion

}
