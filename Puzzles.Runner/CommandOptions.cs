using CommandLine;

namespace Puzzles.Runner;
public class CommandOptions
{
    [Option('e', "examples")]
    public bool Examples { get; set; }

    [Option('y', "year")]
    public int? Year { get; set; }

    [Option('d', "day")]
    public int? Day { get; set; }
}
