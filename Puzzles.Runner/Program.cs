using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puzzles.Web;
using System.Diagnostics;
using System.Reflection;

namespace Puzzles.Runner;

internal class Program
{
    private const string TOKEN_FILE_NAME = "token";

    private static IHost? App { get; set; }
    private static State? State { get; set; }

    static void Main(string[] args)
    {
        try
        {
            ParseArgs(args);
            Init();
            ResolveInput();
            Run(App!.Services.GetRequiredService<IPuzzleSolver>(), State!.PerformanceMode ? 1000 : 1);
        }
        catch (PuzzlesException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private static void Run(IPuzzleSolver solver, int count)
    {        
        if (count > 1)
        {
            Console.WriteLine($"Number of iterations: {count}");
            Console.WriteLine();
        }

        PrintResult("Init", RunWithTime(solver.Init, count));        
        PrintResult("Part 1", RunWithTime(solver.SolvePart1, count));
        PrintResult("Part 2", RunWithTime(solver.SolvePart2, count));
    }

    private static void PrintResult<T>(string title, (double, T) data)
    {
        var(timeMs, result) = data;

        Console.WriteLine(title);
        Console.WriteLine($"Time: {timeMs:f3} ms.");
        Console.WriteLine($"> Answer: {result}");
        Console.WriteLine();
    }

    private static void PrintResult(string title, double timeMs)
    {
        Console.WriteLine(title);
        Console.WriteLine($"Time: {timeMs:f3} ms.");
        Console.WriteLine();
    }

    public static double RunWithTime(Action acton, int count)
    {
        Stopwatch sw = Stopwatch.StartNew();

        for (int i = 0; i < count; i++)
            acton();

        return (sw.Elapsed.TotalMilliseconds / count);
    }

    public static (double timeMs, T result) RunWithTime<T>(Func<T> func, int count)
    {
        Stopwatch sw = Stopwatch.StartNew();
        T result = default!;

        for (int i = 0; i < count; i++)
            result = func();

        return (sw.Elapsed.TotalMilliseconds / count, result);
    }    

    private static void Init()
    {        
        var builder = Host.CreateApplicationBuilder();

        var puzzlesServices = new PuzzlesServices(State!.InputPath);
        puzzlesServices.Register(builder.Services);

        builder.Services.AddTransient(s => new PuzzleLoader(TOKEN_FILE_NAME));
        var name = RegisterSolver(builder.Services);

        Console.WriteLine($"=== {State.Year} Day-{State.Day}: {name} ===");
        Console.WriteLine();
        Console.Error.WriteLine($"Mode: {State.Input}");
        Console.Error.WriteLine($"Input: {Path.GetFullPath(State.InputPath)}");
        Console.Error.WriteLine();

        App = builder.Build();
    }

    private static void ResolveInput()
    {
        if (State!.Input != State.InputMode.Input)
            return;

        if (File.Exists(State.InputPath))
            return;

        Console.Error.WriteLine($"Load input for {State.Year}/{State.Day}...");

        var loader = App!.Services.GetRequiredService<PuzzleLoader>();
        var getInput = loader.GetInput(State.Year, State.Day, CancellationToken.None);
        getInput.Wait();

        File.WriteAllText(State.InputPath, getInput.Result);
        Console.Error.WriteLine($"Input saved to {Path.GetFullPath(State.InputPath)}...");
    }

    private static string RegisterSolver(IServiceCollection services)
    {
        var solverInterface = typeof(IPuzzleSolver);
        var (solver, attribute) = Assembly.GetEntryAssembly()!.GetTypes()
            .Where(solverInterface.IsAssignableFrom)
            .Select(t => 
            (
                type: t, 
                attribute: t.GetCustomAttributes(typeof(PuzzleAttribute), false)
                    .Cast<PuzzleAttribute>()
                    .FirstOrDefault())
            )
            .Where(d => d.attribute is not null && 
                d.attribute.Year == State!.Year && d.attribute.Day == State.Day)
            .Single();

        services.AddTransient(solverInterface, solver);
        return attribute!.Name;
    }

    private static void ParseArgs(string[] args)
    {
        var options = Parser.Default.ParseArguments<CommandOptions>(args)
            .WithParsed(co =>
            {
                var now = DateTime.Now;

                State = new()
                {
                    Day = co.Day ?? now.Day,
                    Year = co.Year ?? now.Year,
                    CustomPath = co.Input,
                    PerformanceMode = co.Performance,
                    Mode = State.RunMode.Run,
                    Input = !String.IsNullOrEmpty(co.Input) ? State.InputMode.Custom : co.Examples ? State.InputMode.Examples : State.InputMode.Input
                };
            })
            .WithNotParsed(_ => throw new PuzzlesException("Arguments are not valid"));
    }
}
