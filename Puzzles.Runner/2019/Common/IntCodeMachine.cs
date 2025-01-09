namespace Puzzles.Runner._2019.Common;

public class IntCodeMachine
{
    private static readonly int[] MODE = [100, 1000, 10000];

    public int[] _init;
    public int[] _memory;

    public List<int> _output = [];
    public Queue<int> _input = [];

    public IntCodeMachine(int[] memory)
    {
        _init = memory;
        _memory = new int[memory.Length];

        Reset();
    }

    public int State { get; private set; }
    public int Size => _memory.Length;
    public IEnumerable<int> Output => _output;

    public int this[int idx] => _memory[idx];

    public int Noun
    {
        get => _memory[1];
        set => _memory[1] = value;
    }

    public int Verb
    {
        get => _memory[2];
        set => _memory[2] = value;
    }

    public int Result => _memory[0];

    public void Reset(int[]? input = null)
    {
        Array.Copy(_init, _memory, _memory.Length);

        _input = new(input ?? []);
        _output.Clear();

        State = 0;
    }

    public void Reset(int noun, int verb, int[]? input = null)
    {
        Reset(input);

        Noun = noun;
        Verb = verb;
    }

    public void Run()
    {
        while(true)
        {
            var op = _memory[State];
            var opCode = OpCode(op);

            if (opCode == 99)
                break;

            State += opCode switch
            {
                1 => Add(Val(op, 1), Val(op, 2), _memory[State + 3]),
                2 => Multiply(Val(op, 1), Val(op, 2), _memory[State + 3]),
                3 => In(_memory[State + 1]),
                4 => Out(_memory[State + 1]),
                _ => throw new InvalidOperationException($"Invalid opcode: {opCode}")
            };
        }
    }

    private int Add(int a, int b, int c)
    {
        _memory[c] = a + b;
        return 4;
    }

    private int Multiply(int a, int b, int c)
    {
        _memory[c] = a * b;
        return 4;
    }


    private int In(int a)
    {
        _memory[a] = _input.Dequeue();
        return 2;
    }
    
    private int Out(int a)
    {
        _output.Add(_memory[a]);
        return 2;
    }

    private int Val(int op, int parameter)
        => V2M(op, parameter) ? _memory[State + parameter] : _memory[_memory[State + parameter]];

    private static int OpCode(int address)
        => address % MODE[0];

    private static bool V2M(int value, int parameter)
        => ((value / MODE[parameter - 1]) & 1) == 1;
}
