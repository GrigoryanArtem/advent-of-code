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
        Run(App.Services.GetRequiredService<IPuzzleSolver>());
    }

    private static void Run(IPuzzleSolver solver)
    {
        var swi = Stopwatch.StartNew();
        solver.Init();
        swi.Stop();

        var sw1 = Stopwatch.StartNew();
        var p1 = solver.SolvePart1();
        sw1.Stop();

        var sw2 = Stopwatch.StartNew();
        var p2 = solver.SolvePart2();
        sw2.Stop();

        Console.WriteLine($"Init time: {swi.ElapsedMilliseconds} ms.");
        Console.WriteLine();

        Console.WriteLine("Part 1:");
        Console.WriteLine($"Time: {sw1.ElapsedMilliseconds} ms.");
        Console.WriteLine($"> Answer: {p1}");
        Console.WriteLine();

        Console.WriteLine("Part 2:");
        Console.WriteLine($"Time: {sw2.ElapsedMilliseconds} ms.");
        Console.WriteLine($"> Answer: {p2}");
    }

    private static void Init()
    {        
        var builder = Host.CreateApplicationBuilder();

        var puzzlesServices = new PuzzlesServices(State.InputPath);
        puzzlesServices.Register(builder.Services);

        var name = RegisterSolver(builder.Services);

        Console.WriteLine($"=== {State.Year} Day-{State.Day}: {name} ===");
        Console.WriteLine();
        Console.WriteLine($"Mode: {State.Mode}");
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
                    Mode = co.Examples ? State.StateMode.Examples : State.StateMode.Input
                };
            })
            .WithNotParsed(_ => throw new ArgumentException());
    }
}
