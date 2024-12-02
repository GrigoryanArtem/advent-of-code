const string INPUT = "input.txt";
const string EXAMPLE = "example.txt";

const string FILE = INPUT;

const int MIN = 1;
const int MAX = 3;

int p1Counter = 0;
int p2Counter = 0;

foreach (var line in File.ReadAllLines(FILE))
{
    var tokens = line.Split([' '], StringSplitOptions.RemoveEmptyEntries)
        .Select(t => Convert.ToInt32(t.Trim()))
        .ToArray();

    var mat = CreateDiffMatrix(tokens, 2);

    var p1Success = IsPathExist(mat, tokens.Length - 1, 0);
    var p2Success = p1Success || IsPathExist(mat, tokens.Length - 1, 1) ||
        IsPathExist(mat, tokens.Length - 1, 0, 1) || IsPathExist(mat, tokens.Length - 2, 0);

    p1Counter += p1Success ? 1 : 0;
    p2Counter += p2Success ? 1 : 0;    
}

Console.WriteLine($"part 1: {p1Counter}");
Console.WriteLine($"part 2: {p2Counter}");

bool IsPathExist(int?[,] mat, int index, int mistakes, int distance = 0, int direction = 0)
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

        if(!safe)
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

int?[,] CreateDiffMatrix(int[] nums, int distance)
{
    int?[,] result = new int?[nums.Length, distance];

    for(int i = 1; i < nums.Length; i++)
        for (int k = 0; k < distance; k++)
            result[i, k] = i - (k + 1) >= 0 ? nums[i] - nums[i - (k + 1)] : null;

    return result;
}