namespace Puzzles.Base.Abstractions;

public interface ILinesInputReader
{
    string[] Lines { get; }

    T[][] GetTokens<T>(string separator, Func<string, T> converter);
    T[] Convert<T>(Func<string, T> converter);
}
