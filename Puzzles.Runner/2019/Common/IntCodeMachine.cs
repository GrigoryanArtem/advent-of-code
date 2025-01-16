namespace Puzzles.Runner._2019.Common;

public class IntCodeMachine
{
    private static readonly long[] MODE = [100, 1000, 10000];

    #region Members

    public long[] _init;
    public long[] _memory;

    private bool _inputWaiting;

    public List<long> _output = [];
    public Queue<long> _input = [];

    #endregion

    public IntCodeMachine(long[] memory)
    {
        _init = memory;
        _memory = new long[memory.Length];

        Reset();
    }

    public long State { get; private set; }
    public int Size => _memory.Length;
    public IEnumerable<long> Output => _output;

    public bool Halted { get; private set; }

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

    public long Result => _memory[0];

    public void Reset(long[]? input = null)
    {
        Array.Copy(_init, _memory, _memory.Length);

        _input = new(input ?? []);
        _output.Clear();

        State = 0;
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

        while (!_inputWaiting)
        {
            var op = _memory[State];
            var opCode = OpCode(op);

            Halted = opCode == 99;
            if (Halted)
                break;

            State = opCode switch
            {
                1 => Add(Val(1), Val(2), Ref(3)),
                2 => Multiply(Val(1), Val(2), Ref(3)),

                3 => In(Ref(1)),
                4 => Out(Ref(1)),

                5 => JIT(Val(1), Val(2)),
                6 => JIF(Val(1), Val(2)),
                7 => LN(Val(1), Val(2), Ref(3)),
                8 => EQ(Val(1), Val(2), Ref(3)),

                _ => throw new InvalidOperationException($"Invalid opcode: {opCode}")
            };
        }
    }

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
        _output.Add(_memory[a]);
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
        _memory[c] = a < b ? 1 : 0;
        return State + 4;
    }

    // equals
    private long EQ(long a, long b, long c)
    {
        _memory[c] = a == b ? 1 : 0;
        return State + 4;
    }

    #endregion

    #region Additional methods

    private long Ref(long parameter)
        => _memory[State + parameter];

    private long Val(int parameter)
        => V2M(_memory[State], parameter) ? _memory[State + parameter] : _memory[_memory[State + parameter]];

    private static long OpCode(long address)
        => address % MODE[0];

    private static bool V2M(long value, int parameter)
        => ((value / MODE[parameter - 1]) & 1) == 1;

    #endregion
}
