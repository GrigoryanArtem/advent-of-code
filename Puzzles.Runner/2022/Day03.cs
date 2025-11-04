namespace Puzzles.Runner._2022;

[Puzzle("Rucksack Reorganization", 3, 2022)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => input.Lines.Sum(line => Prioritiy(line.Chunk(line.Length / 2))).ToString();

    public string SolvePart2()
        => input.Lines.Chunk(3).Sum(Prioritiy).ToString();

    private static int Prioritiy(IEnumerable<IEnumerable<char>> rucksacks)
    {
        var set = rucksacks.First().ToHashSet();
        foreach (var chank in rucksacks.Skip(1))
            set.IntersectWith(chank);

        return set.Sum(P);
    }

    private static int P(char ch) 
        => ch < 'a' ? ch - 'A' + 27 : ch - 'a' + 1;
}
