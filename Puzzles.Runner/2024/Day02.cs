namespace Puzzles.Runner._2024;

[Puzzle("Red-Nosed Reports", 2, 2024)]
public partial class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private const int MIN = 1;
    private const int MAX = 3;

    private int[][] _lines = [];

    public void Init()
        => _lines = input.GetTokens(" ", Convert.ToInt32);

    public string SolvePart1()
    {
        int counter = 0;

        foreach (var tokens in _lines)
        {
            var mat = CreateDiffMatrix(tokens, 2);
            var success = IsPathExist(mat, tokens.Length - 1, 0);

            if (success)
                counter++;
        }

        return counter.ToString();
    }

    public string SolvePart2()
    {
        int counter = 0;

        foreach (var tokens in _lines)
        {
            var mat = CreateDiffMatrix(tokens, 2);
            var success = IsPathExist(mat, tokens.Length - 1, 1) ||
                IsPathExist(mat, tokens.Length - 1, 0, 1) || IsPathExist(mat, tokens.Length - 2, 0);

            if (success)
                counter++;
        }

        return counter.ToString();
    }

    #region Additional

    private static bool IsPathExist(int?[,] mat, int index, int mistakes, int distance = 0, int direction = 0)
    {
        if (index >= mat.GetLength(0) || distance >= mat.GetLength(1))
            return false;

        var success = true;

        while (success)
        {
            var diff = mat[index, distance];

            if (!diff.HasValue)
                break;

            var (sign, abs) = (Math.Sign(diff.Value), Math.Abs(diff.Value));

            var signCorrect = (direction == 0 || sign == direction);
            var safe = signCorrect && abs >= MIN && abs <= MAX;

            if (!safe)
            {
                return mistakes > 0 && (IsPathExist(mat, index, mistakes - 1, distance + 1, direction) ||
                    IsPathExist(mat, index + 1, mistakes - 1, distance + 1, direction));
            }

            index -= (distance + 1);
            distance = 0;
            direction = sign;
        }

        return success;
    }

    private static int?[,] CreateDiffMatrix(int[] numbers, int distance)
    {
        int?[,] result = new int?[numbers.Length, distance];

        for (int i = 1; i < numbers.Length; i++)
            for (int k = 0; k < distance; k++)
                result[i, k] = i - (k + 1) >= 0 ? numbers[i] - numbers[i - (k + 1)] : null;

        return result;
    }

    #endregion
}
