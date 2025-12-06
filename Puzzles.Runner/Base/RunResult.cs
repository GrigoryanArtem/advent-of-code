namespace Puzzles.Runner.Base;

public record RunResult<T>(double TimeMs, int Iterations, T Result) : 
    BaseRunResult(TimeMs, Iterations);
