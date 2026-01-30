namespace Puzzles.Runner._2017;

[Puzzle("Inverse Captcha", 1, 2017)]
public class Day01(IFullInputReader input) : IPuzzleSolver
{
    private readonly string _text = input.Text;

    public string SolvePart1()
        => CalcaulteCapcha(1).ToString();

    public string SolvePart2()
        => CalcaulteCapcha(_text.Length / 2).ToString();

    public int CalcaulteCapcha(int shift)
    {
        var sum = 0;

        for (int i = 0; i < _text.Length; i++)
            if (_text[i] == _text[AOC.Mod(i + shift, _text.Length)])
                sum += _text[i] - '0';

        return sum;
    }
}
