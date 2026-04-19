namespace Puzzles.Runner._2016;

[Puzzle("Signals and Noise", 6, 2016)]
public partial class Day06(ILinesInputReader input) : IPuzzleSolver
{
    private int _size = 0;
    private int[][] _frequencies = [];

    public void Init()
    {
        _size = input.Lines.First().Length;
        _frequencies = new int[_size][];

        for (int i = 0; i < _size; i++)
            _frequencies[i] = new int[26];

        foreach (var line in input.Lines)
            for (int i = 0; i < _size; i++)
                _frequencies[i][C2I(line[i])]++;
    }

    public string SolvePart1()
    {
        Span<char> answer = stackalloc char[_size];

        for(int i = 0; i < _frequencies.Length; i++)                    
            answer[i] = I2C(_frequencies[i].IndexOfMax());
        
        return new string(answer);
    }

    public string SolvePart2()
    {
        Span<char> answer = stackalloc char[_size];

        for (int i = 0; i < _size; i++)
        {
            var min = _frequencies[i].IndexOfMin(d => d > 0 ? d : Int32.MaxValue);
            answer[i] = I2C(min);
        }

        return new string(answer);
    }

    private static int C2I(char ch)
        => ch - 'a';

    private static char I2C(int id)
        => (char)(id + 'a');
}