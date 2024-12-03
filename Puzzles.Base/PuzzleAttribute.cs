namespace Puzzles.Base;

[AttributeUsage(AttributeTargets.Class)]
public class PuzzleAttribute(string name, int day, int year) : Attribute
{
    public string Name { get; } = name;

    public int Day { get; } = day;
    public int Year { get; } = year;
}
