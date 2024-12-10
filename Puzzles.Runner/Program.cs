using CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Reflection;

namespace Puzzles.Runner;

internal class Program
{
    private static IHost App { get; set; }
    private static State State { get; set; }

    static void Main(string[] args)
    {
        ParseArgs(args);
        Init();
        Run(App.Services.GetRequiredService<IPuzzleSolver>(), State.PerformanceMode ? 1000 : 1);        
    }

    private static void Run(IPuzzleSolver solver, int count)
    {
        var initTime = Stopwatch.StartNew();
        solver.Init();
        initTime.Stop();

        Console.WriteLine($"Init time: {initTime.Elapsed.TotalMilliseconds} ms.");
        Console.WriteLine();

        if (count > 1)
        {
            Console.WriteLine($"Number of iterations: {count}");
            Console.WriteLine();
        }

        var part1 = RunWithTime(solver.SolvePart1, count);
        Console.WriteLine("Part 1:");
        Console.WriteLine($"Time: {part1.timeMs:f3} ms.");
        Console.WriteLine($"> Answer: {part1.result}");
        Console.WriteLine();

        var part2 = RunWithTime(solver.SolvePart2, count);
        Console.WriteLine("Part 2:");
        Console.WriteLine($"Time: {part2.timeMs:f3} ms.");
        Console.WriteLine($"> Answer: {part2.result}");
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

        var puzzlesServices = new PuzzlesServices(State.InputPath);
        puzzlesServices.Register(builder.Services);

        var name = RegisterSolver(builder.Services);

        Console.WriteLine($"=== {State.Year} Day-{State.Day}: {name} ===");
        Console.WriteLine();
        Console.WriteLine($"Mode: {State.Input}");
        Console.WriteLine($"Input: {Path.GetFullPath(State.InputPath)}");
        Console.WriteLine();

        App = builder.Build();
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
                d.attribute.Year == State.Year && d.attribute.Day == State.Day)
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
                    Input = !String.IsNullOrEmpty(co.Input) ? State.InputMode.Custom : co.Examples ? State.InputMode.Examples : State.InputMode.Input
                };
            })
            .WithNotParsed(_ => throw new ArgumentException());
    }
}
