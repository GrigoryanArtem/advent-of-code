using Microsoft.Extensions.DependencyInjection;
using Puzzles.Base.Abstractions;

namespace Puzzles.Base;

public class PuzzlesServices(string inputPath)
{
    public void Register(IServiceCollection services)
    {
        services.AddTransient<IFullInputReader, FullInputReader>(_ => new(inputPath));
        services.AddTransient<ILinesInputReader, LinesInputReader>(_ => new(inputPath));
    }
}
