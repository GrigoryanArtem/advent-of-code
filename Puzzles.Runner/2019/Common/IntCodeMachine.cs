namespace Puzzles.Runner._2019.Common;

public class IntCodeMachine
{
    public int[] _init;    
    public int[] _memory;    

    public IntCodeMachine(int[] memory)
    {
        _init = memory;
        _memory = new int[memory.Length];

        Reset();
    }

    public int Size => _memory.Length;

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

    public void Reset()
        => Array.Copy(_init, _memory, _memory.Length);

    public void Reset(int noun, int verb)
    {
        Array.Copy(_init, _memory, _memory.Length);

        Noun = noun;
        Verb = verb;
    }

    public void Run()
    {
        for (int i = 0; i < _memory.Length; i += 4)
        {
            var op = _memory[i];

            if (op == 99)
                break;

            RunOp(op, _memory[_memory[i + 1]], _memory[_memory[i + 2]], ref _memory[_memory[i + 3]]);
        }
    }

    private static void RunOp(int op, int a, int b, ref int r)
    {
        switch (op)
        {
            case 1:
                Add(a, b, ref r);
                break;
            case 2:
                Multiply(a, b, ref r);
                break;
        }
    }

    private static void Add(int a, int b, ref int r)
        => r = a + b;

    private static void Multiply(int a, int b, ref int r)
        => r = a * b;
}
