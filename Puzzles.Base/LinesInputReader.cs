using Puzzles.Base.Abstractions;

namespace Puzzles.Base;

internal class LinesInputReader(string path) : ILinesInputReader
{
    public string[] Lines { get; } = File.ReadAllLines(path);
    public T[][] GetTokens<T>(string separator, Func<string, T> converter)
        => Lines.Select(line => line.Split(separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => converter(t.Trim()))
                .ToArray())
            .ToArray();
}
