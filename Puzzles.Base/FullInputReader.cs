using Puzzles.Base.Abstractions;

namespace Puzzles.Base;

public class FullInputReader(string path) : IFullInputReader
{
    public string Text { get; } = File.ReadAllText(path);
}
