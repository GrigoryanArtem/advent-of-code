namespace Puzzles.Runner;
public class State
{
    public enum StateMode
    {
        Input,
        Examples
    }

    public int Year { get; set; }
    public int Day { get; set; }    

    public StateMode Mode { get; set; }
    public string InputPath => @$"{Year}/{ModeToPath()}/{Day}.in";

    private string ModeToPath() => Mode switch
    {
        StateMode.Input => "input",
        StateMode.Examples => "examples",
        _ => throw new NotImplementedException("Mode not implemented")
    };
}
