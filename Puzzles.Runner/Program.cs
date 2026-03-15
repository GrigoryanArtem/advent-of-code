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
            .AddRow("1", p1.Answer!)
            .AddRow("2", p2.Answer!)
            .Build();

        Console.WriteLine(answerTable);

        Dictionary<string, IPuzzleStatistic> results = new()
        {
            ["Init"] = init,
            ["Part 1"] = p1,
            ["Part 2"] = p2
        };

        var timeTable = TableBuilder.Create(TableOptions.All)
            .SetMargin(1)
            .AddColumn(new() { Header = String.Empty })
            .AddColumn(new() { Header = "ITER", Align = Align.Right })
            .AddColumn(new() { Header = "P90", Align = Align.Right, Format = "{0:f3}" })
            .AddColumn(new() { Header = "P95", Align = Align.Right, Format = "{0:f3}" })
            .AddColumn(new() { Header = "P99", Align = Align.Right, Format = "{0:f3}" })
            .AddColumn(new() { Header = "STDDEV", Align = Align.Right, Format = "{0:f3}" })
            .AddColumn(new() { Header = "MEAN", Align = Align.Right, Format = "{0:f3}" })
            .AddColumn(new() { Header = "MEDIAN", Align = Align.Right, Format = "{0:f3}" });

        foreach (var (name, result) in results)
            timeTable.AddRow(name, result.Iterations, result.P90, result.P95, result.P99, result.StdDev, result.Mean, result.Median);
        
        timeTable.AddRow(
            "Total", 
            String.Empty,
            String.Empty, 
            String.Empty, 
            String.Empty,
            String.Empty, 
            results.Values.Sum(v => v.Mean),
            results.Values.Sum(v => v.Median)
        );

        Console.WriteLine(timeTable.Build());

        ConsoleHistogram ch = new(70, 10, 2, "ms.");

        Console.WriteLine("> PART 1 HISTOGRAM");
        Console.WriteLine();
        ch.Draw(p1.Data);
        Console.WriteLine();

        Console.WriteLine("> PART 2 HISTOGRAM");
        Console.WriteLine();
        ch.Draw(p2.Data);
        Console.WriteLine();
    }

    public static PuzzleStatistic<object> RunWithTime(Action action, int count)
    {
        var times = new List<double>(count);

        var sw = Stopwatch.StartNew();
        var total = Stopwatch.StartNew();

        for (int i = 0; i < count && total.Elapsed.Seconds < TIME_THRESHOLD_SEC; i++)
        {
            sw.Restart();
            action();
            sw.Stop();

            times.Add(sw.Elapsed.TotalMilliseconds);
        }
                
        return PuzzleStatistic<object>.Create(times);
    }

    public static PuzzleStatistic<T> RunWithTime<T>(Func<T> func, int count)
    {
        var times = new List<double>(count);

        var sw = Stopwatch.StartNew();
        var total = Stopwatch.StartNew();

        T answer = default!;

        for (int i = 0; i < count && total.Elapsed.Seconds < TIME_THRESHOLD_SEC; i++)
        {
            sw.Restart();
            answer = func();
            sw.Stop();

            times.Add(sw.Elapsed.TotalMilliseconds);
        }

        return PuzzleStatistic<T>.Create(times, answer);
    }

    private static void Init()
    {
        var builder = Host.CreateApplicationBuilder();

        var runInfo = new RunInfo() { IsExample = State!.Input == State.InputMode.Examples };
        builder.Services.AddSingleton<IRunInfo>(runInfo);

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
            .Single(d => d.attribute is not null &&
                d.attribute.Year == State!.Year && d.attribute.Day == State.Day);

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
