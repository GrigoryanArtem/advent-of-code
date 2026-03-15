namespace Puzzles.Runner._2015;

[Puzzle("Perfectly Spherical Houses in a Vacuum", 3, 2015)]
public class Day03(IFullInputReader input) : IPuzzleSolver
{
    private readonly Dictionary<char, Vec2> _dirs = new()
    {
        ['>'] = new(1, 0),
        ['<'] = new(-1, 0),
        ['^'] = new(0, 1),
        ['v'] = new(0, -1),
    };

    public string SolvePart1()
        => Solve(1).ToString();

    public string SolvePart2() 
        => Solve(2).ToString();

    private int Solve(int numberOfActors)
    {
        Span<Vec2> actors = stackalloc Vec2[numberOfActors];
        HashSet<Vec2> visited = new(input.Text.Length + 1)
        {
            Vec2.Zero
        };

        for (int i = 0; i < input.Text.Length; i++)
        {
            ref var actor = ref actors[i % numberOfActors];
            actor.Move(_dirs[input.Text[i]]);            
            visited.Add(actor);
        }

        return visited.Count;
    }
}
