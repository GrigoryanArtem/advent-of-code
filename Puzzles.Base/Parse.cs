namespace Puzzles.Base;

public static class Parse
{
    public static IEnumerable<int> StringToNumbers(string str)
        => StringToNumbers(str, 0, str.Length);

    public static IEnumerable<int> StringToNumbers(string str, int start, int end)
    {
        var curr = 0;
        var sign = 1;
        var hasDigit = false;

        for (int i = start; i < end; i++)
        {
            var ch = str[i];

            if (Char.IsDigit(str[i]))
            {
                curr = (curr * 10) + (ch - '0');
                hasDigit = true;
            }
            else if (ch == '-' && !hasDigit)
            {
                sign = -1;
            }
            else if (hasDigit)
            {
                yield return curr * sign;

                curr = 0;
                sign = 1;
                hasDigit = false;
            }
            else
            {
                sign = 1;
            }
        }

        if (hasDigit)
            yield return curr * sign;
    }
}
