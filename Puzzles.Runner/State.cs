namespace Puzzles.Runner;
public class State
{
    public enum InputMode
    {
        Input,
        Examples,
        Custom
    }

    public int Year { get; set; }
    public int Day { get; set; }    

    public bool PerformanceMode { get; set; }

    public InputMode Input { get; set; }

    public string? CustomPath { get; set; }
    public string InputPath => Input == InputMode.Custom ? CustomPath! : @$"{Year}/{ModeToPath()}/{Day}.in";

    private string ModeToPath() => Input switch
    {
        InputMode.Input => "input",
        InputMode.Examples => "examples",
        _ => throw new NotImplementedException("Mode not implemented")
    };
}
