const string INPUT = "input.txt";
const string EXAMPLE = "example .txt";

const string FILE = INPUT;

const int MIN = 1;
const int MAX = 3;

int counter = 0;
foreach (var line in File.ReadAllLines(FILE))
{
    var tokens = line.Split([' '], StringSplitOptions.RemoveEmptyEntries)
        .Select(t => Convert.ToInt32(t.Trim()))
        .ToArray();

    int direction = 0;
    int corrections = 1;
    bool safe = true;
    for (int i = 1; i < tokens.Length && safe;)
    {
        (safe, i) = IsSafe(tokens, i, ref direction, ref corrections);
    }

    if(safe)
        counter++;
}

Console.WriteLine($"answer: {counter}");

(bool, int) IsSafe(int[] tokens, int idx, ref int direction, ref int corrections)
{  
    var (safe, sign) = Check(tokens[idx - 1] - tokens[idx], direction);    
    var next= idx + 1;

    if (!safe && corrections > 0 && idx - 2 >= 0)
    {        
        (safe, sign) = Check(tokens[idx - 2] - tokens[idx], direction);     
        if (safe) corrections--;
    }

    if (!safe && corrections > 0 && idx + 1 < tokens.Length)
    {        
        (safe, sign) = Check(tokens[idx - 1] - tokens[idx + 1], direction);
        corrections--;
        next = idx + 2;
    }

    direction = safe ? sign : direction;
    return (safe, next);
}

(bool safe, int sign) Check(int diff, int direction)
{
    var (sign, abs) = (Math.Sign(diff), Math.Abs(diff));
    var signCorrect = (direction == 0 || sign == direction);
    var safe = signCorrect && abs >= MIN && abs <= MAX;

    return (safe, sign);
}