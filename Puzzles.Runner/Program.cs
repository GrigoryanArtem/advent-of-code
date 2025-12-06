using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Puzzles.Runner.Base;
using Puzzles.Runner.Base.Table;
using Puzzles.Web;
using System.Diagnostics;
using System.Reflection;

namespace Puzzles.Runner;

internal class Program
{
    private const string TOKEN_FILE_NAME = "token";
    private const int TIME_THRESHOLD_SEC = 10;
    private const int PERFORMANCE_MODE_ITERATIONS = 10000;

    private static IHost? App { get; set; }
    private static State? State { get; set; }

    static void Main(string[] args)
    {
        try
        {
            ParseArgs(args);
            Init();
            ResolveInput();
            Run(App!.Services.GetRequiredService<IPuzzleSolver>(), State!.PerformanceMode ? PERFORMANCE_MODE_ITERATIONS : 1);
        }
        catch (PuzzlesException ex)
        {
            Console.Error.WriteLine(ex.Message);
        }
    }

    private static void Run(IPuzzleSolver solver, int count)
    {   
        var init = RunWithTime(solver.Init, count);
        var p1 = RunWithTime(solver.SolvePart1, count);
        var p2 = RunWithTime(solver.SolvePart2, count);

        var answerTable = TableBuilder.Create(TableOptions.All)
            .SetMargin(1)
            .AddColumn(new() { Header = "#" })
            .AddColumn(new() { Header = "Answer", Align = Align.Right })
            .AddRow("1", p1.Result)
            .AddRow("2", p2.Result)
            .Build();

        Console.WriteLine(answerTable);

        Dictionary<string, BaseRunResult> results = new()
        {
            ["Init"] = init,
            ["Part 1"] = p1,
            ["Part 2"] = p2
        };

        var timeTable = TableBuilder.Create(TableOptions.All)
            .SetMargin(1)
            .AddColumn(new() { Header = "" })
            .AddColumn(new() { Header = "Iterations", Align = Align.Right })
            .AddColumn(new() { Header = "Time, ms", Align = Align.Right, Format = "{0:f3}" });

        foreach (var (name, result) in results)        
            timeTable.AddRow(name, result.Iterations, result.TimeMs);

        timeTable.AddRow("Total", String.Empty, results.Values.Sum(v => v.TimeMs));            

        Console.WriteLine(timeTable.Build());
    }

    public static BaseRunResult RunWithTime(Action action, int count)
    {
        var sw = new Stopwatch();

        var iteration = 0;
        for (; iteration < count && sw.Elapsed.Seconds < TIME_THRESHOLD_SEC; iteration++)
        {
            sw.Start();
            action();
            sw.Stop();
        }

        return new(sw.Elapsed.TotalMilliseconds / iteration, iteration);
    }

    public static RunResult<T> RunWithTime<T>(Func<T> func, int count)
    {
        var sw = new Stopwatch();
        T result = default!;

        var iteration = 0;
        for (; iteration < count && sw.Elapsed.Seconds < TIME_THRESHOLD_SEC; iteration++)
        {
            sw.Start();
            result = func();
            sw.Stop();
        }

        return new(sw.Elapsed.TotalMilliseconds / iteration, iteration, result);
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
