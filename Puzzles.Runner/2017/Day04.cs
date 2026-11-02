namespace Puzzles.Runner._2017;

[Puzzle("High-Entropy Passphrases", 4, 2017)]
public class Day04(ILinesInputReader input) : IPuzzleSolver
{
    private string[][] _passphrases = [];
    private int[] _buffer = new int[26];

    public void Init()
        => _passphrases = input.Convert(str => str.Split(' ', StringSplitOptions.RemoveEmptyEntries));
    
    public string SolvePart1()
        => CountValidPassphrases(_passphrases, x => x).ToString();    

    public string SolvePart2()
        => CountValidPassphrases(_passphrases, GetStringFingerprint).ToString();

    private static int CountValidPassphrases<T>(string[][] passphrases, Func<string, T> fingerprintFunction)
    {
        HashSet<T> set = [];

        var count = 0;
        foreach (var passphrase in passphrases)
        {
            set.Clear();

            var valid = true;
            foreach (var word in passphrase)
            {
                var mapped = fingerprintFunction(word);
                if (!set.Add(mapped))
                {
                    valid = false;
                    break;
                }
            }

            if (valid)
                count++;
        }

        return count;
    }

    private String GetStringFingerprint(string word)
        => String.Create(26, word, static (counts, source) =>
        {
            counts.Clear();

            foreach (var ch in source)
                counts[ch - 'a']++;
        });
}
