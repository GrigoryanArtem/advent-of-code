namespace Puzzles.Base.Abstractions;

public interface IPuzzleSolver
{
    void Init() { }

    string SolvePart1();
    string SolvePart2() => "NOT CALCULATED";
}
