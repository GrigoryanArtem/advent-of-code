namespace Puzzles.Runner._2017;

[Puzzle("Spiral Memory", 3, 2017)]
public class Day03(IFullInputReader input) : IPuzzleSolver
{
    private static readonly Vec2[] _directions =
    [
        new(0, 1),
        new(-1, 0),
        new(0, -1),
        new(1, 0),
        new(1, 0)
    ];

    private static readonly Vec2[] _adjacent =
    [
        new(-1, -1),
        new(0, -1),
        new(1, -1),
        new(-1, 0),
        new(1, 0),
        new(-1, 1),
        new(0, 1),
        new(1, 1)
    ];

    private int _target = 0;

    public void Init()
        => _target = Convert.ToInt32(input.Text);    

    public string SolvePart1()
    {        
        var sqrt = (int)Math.Sqrt(_target);
        var layer = (sqrt + 1) / 2;        
        var side = (layer * 2);

        var max = (side + 1) * (side + 1);
        var offset = (max - _target) % side;
        var distance = layer + Math.Abs(offset - layer);

        return distance.ToString();
    }

    public string SolvePart2()
    {
        var grid = new Dictionary<Vec2, int>()
        { 
            [Vec2.Zero] = 1 
        };

        var pos = new Vec2(1, 0);        
        for (int ring = 1; ring < 1024 ; ring++)
        {
            var side = ring * 2;
            var count = side * 4;

            for (int i = 0; i < count; i++)
            {
                var sum = _adjacent.Sum(a => grid.GetValueOrDefault(pos + a, 0));

                if (sum > _target)
                    return sum.ToString();

                grid[pos] = sum;
                pos += _directions[(i + 1) / side];
            }
        }

        return AOC.NO_ANSWER;
    }
}
