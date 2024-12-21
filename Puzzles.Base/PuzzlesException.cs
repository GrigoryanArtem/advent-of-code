namespace Puzzles.Base;

public class PuzzlesException : Exception
{
    public PuzzlesException(){ }
    public PuzzlesException(string? message) : base(message) { }
    public PuzzlesException(string? message, Exception? innerException) : base(message, innerException) { }
}
