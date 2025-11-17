namespace Puzzles.Runner._2021;

using Card = Mat2<int>;

[Puzzle("Giant Squid", 4, 2021)]
public class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private const int MAT_SIZE = 5;

    private int[] _nums = [];
    private Card[] _cards = [];

    public void Init()
    {
        _nums = [.. input.Lines[0].Split(',').Select(Int32.Parse)];
        _cards = [.. input.Lines.Skip(1)
            .Where(line => !String.IsNullOrEmpty(line))
            .Chunk(MAT_SIZE)
            .Select(chunk => new Card([..chunk
                .SelectMany(line => line.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(Int32.Parse))], MAT_SIZE))];
    }

    public string SolvePart1()
        => CalculateWinValues().First().ToString();

    public string SolvePart2()
        => CalculateWinValues().Last().ToString();

    private IEnumerable<int> CalculateWinValues()
    {
        HashSet<int> played = [.. _nums.Take(MAT_SIZE)];
        HashSet<Card> win = [];

        for (int idx = MAT_SIZE; idx < _nums.Length; idx++)
        {
            played.Add(_nums[idx]);

            foreach (var card in _cards.Where(c => !win.Contains(c)))
            {                
                for (int i = 0; i < MAT_SIZE; i++)
                {
                    if (card.Row(i).All(played.Contains) || card.Column(i).All(played.Contains))
                    {
                        win.Add(card);
                        var value = card.Data.Sum(d => played.Contains(d) ? 0 : d);
                        yield return value * _nums[idx];
                        break;
                    }
                }
            }
        }
    }
}
