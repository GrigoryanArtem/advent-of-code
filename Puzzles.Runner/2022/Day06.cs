namespace Puzzles.Runner._2022;

[Puzzle("Tuning Trouble", 6, 2022)]
public class Day06(IFullInputReader input) : IPuzzleSolver
{
    private class UniqueBuffer()
    {
        private readonly short[] _buffer = new short[Char.MaxValue];
        private int _notUnique = 0;

        public bool AllUnique => _notUnique == 0;

        public void Clear()
        {
            Array.Clear(_buffer);
            _notUnique = 0;
        }

        public void Push(char ch)
        {
            _notUnique += _buffer[ch] == 1 ? 1 : 0;
            _buffer[ch]++;
        }

        public void Pop(char ch)
        {
            _buffer[ch]--;
            _notUnique -= _buffer[ch] == 1 ? 1 : 0;
        }
    }

    private readonly UniqueBuffer _buffer = new();

    public string SolvePart1()
        => FindMarker(input.Text, 4).ToString();

    public string SolvePart2()
        => FindMarker(input.Text, 14).ToString();

    private int FindMarker(string text, int len)
    {
        _buffer.Clear();

        int idx = 0;
        for (;idx < len; idx++)
            _buffer.Push(text[idx]);

        for (;idx < text.Length && !_buffer.AllUnique; idx++)
        {
            _buffer.Pop(text[idx - len]);
            _buffer.Push(text[idx]);
        }

        return idx;
    }
}
