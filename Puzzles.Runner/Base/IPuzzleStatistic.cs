namespace Puzzles.Runner.Base;

public interface IPuzzleStatistic
{
    double[] Data { get; }
    int Iterations { get; }
    double Mean { get; }
    double Median { get; }
    double StdDev { get; }
    double P90 { get; }
    double P95 { get; }
    double P99 { get; }
}
