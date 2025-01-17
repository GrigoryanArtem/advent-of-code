namespace Puzzles.Runner._2019.Common;

public class IntCodeMachine
{
    private enum Mode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }

    private static readonly long[] MODE_MASK = [100, 1000, 10000];

    public static IntCodeMachine Null => new([]);

    #region Members

    private readonly long[] _init;
    private readonly long[] _memory;

    private bool _inputWaiting;

    private readonly List<long> _output = [];
    private Queue<long> _input = [];

    #endregion

    public IntCodeMachine(long[] memory, int? memorySize = null)
    {
        _init = memory;
        _memory = new long[memorySize ?? memory.Length];

        Reset();
    }

    #region Properties

    public long this[int idx] => _memory[idx];

    public long Noun
    {
        get => _memory[1];
        set => _memory[1] = value;
    }

    public long Verb
    {
        get => _memory[2];
        set => _memory[2] = value;
    }

    public long State { get; private set; }
    public int MemorySize => _memory.Length;
    public IEnumerable<long> Output => _output;

    public bool Halted { get; private set; }
    public long RelativeBase { get; private set; }

    #endregion

    public void Reset(long[]? input = null)
    {
        Array.Clear(_memory, 0, _memory.Length);
        Array.Copy(_init, _memory, _init.Length);

        _input = new(input ?? []);
        _output.Clear();

        RelativeBase = 0;
        State = 0;
        Halted = false;
    }

    public void Reset(long noun, long verb, long[]? input = null)
    {
        Reset(input);

        Noun = noun;
        Verb = verb;
    }

    public void Input(long value)
        => _input.Enqueue(value);

    public void Run()
    {
        _inputWaiting = false;

        while (!_inputWaiting && !Halted)
        {
            var op = _memory[State];
            var opCode = OpCode(op);
            
            if (Halted = opCode == 99)
                break;

            State = opCode switch
            {
                1 => Add(Val(1), Val(2), Ref(3)),
                2 => Multiply(Val(1), Val(2), Ref(3)),

                3 => In(Ref(1)),
                4 => Out(Val(1)),

                5 => JIT(Val(1), Val(2)),
                6 => JIF(Val(1), Val(2)),
                7 => LN(Val(1), Val(2), Ref(3)),
                8 => EQ(Val(1), Val(2), Ref(3)),

                9 => ARB(Val(1)),

                _ => throw new InvalidOperationException($"Invalid opcode: {opCode}")
            };
        }
    }

    public static IntCodeMachine FromInput(ILinesInputReader input, int? memorySize = null)
        => new([.. input.GetTokens(",", Convert.ToInt64).First()], memorySize);

    #region Private methods    

    #region Math

    private long Add(long a, long b, long c)
    {
        _memory[c] = a + b;
        return State + 4;
    }

    private long Multiply(long a, long b, long c)
    {
        _memory[c] = a * b;
        return State + 4;
    }

    #endregion

    #region I/O

    private long In(long a)
    {
        if (_input.TryDequeue(out var value))
        {
            _memory[a] = value;
            return State + 2;
        }

        _inputWaiting = true;
        return State;
    }

    private long Out(long a)
    {
        _output.Add(a);
        return State + 2;
    }

    #endregion

    #region Jumps

    // jump-if-true
    private long JIT(long a, long b)
        => a != 0L ? b : State + 3;

    // jump-if-false
    private long JIF(long a, long b)
        => a == 0L ? b : State + 3;

    // less than
    private long LN(long a, long b, long c)
    {
        _memory[c] = a < b ? 1L : 0L;
        return State + 4;
    }

    // equals
    private long EQ(long a, long b, long c)
    {
        _memory[c] = a == b ? 1L : 0L;
        return State + 4;
    }

    #endregion

    #region Relative base

    // adjusts the relative base
    public long ARB(long a)
    {
        RelativeBase += a;
        return State + 2;
    }

    #endregion

    #region Additional methods

    private long Ref(int parameter) => V2M(_memory[State], parameter) switch
    {
        Mode.Position => _memory[State + parameter],
        Mode.Immediate => State + parameter,
        Mode.Relative => RelativeBase + _memory[State + parameter],

        _ => throw new NotSupportedException()
    };

    private long Val(int parameter)
        => _memory[Ref(parameter)];

    private static long OpCode(long address)
        => address % MODE_MASK[0];

    private static Mode V2M(long value, int parameter)
        => (Mode)(value / MODE_MASK[parameter - 1] % 10);

    #endregion

    #endregion
}
