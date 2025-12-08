namespace Puzzles.Base.Entities;

public ref struct SpanHeap<T> where T : IComparable<T>
{
    private readonly Span<T> _data;

    public SpanHeap(Span<T> data)
    {
        _data = data;
        Count = data.Length;

        for (int i = (Count >> 1) - 1; i >= 0; i--)
            SiftDown(i);
    }

    public int Count { get; private set; }

    public T Pop()
    {
        T result = _data[0];        
        _data[0] = _data[--Count];

        int i = 0;
        while (true)
        {
            int left = (i << 1) + 1;
            int right = left + 1;

            if (left >= Count)
                break;

            int smallest = right < Count && _data[right].CompareTo(_data[left]) < 0
                ? right
                : left;

            if (_data[i].CompareTo(_data[smallest]) <= 0)
                break;

            Swap(i, smallest);
            i = smallest;
        }

        return result;
    }

    private readonly void SiftDown(int i)
    {
        while (true)
        {
            int left = (i << 1) + 1;
            if (left >= Count)
                return;

            int right = left + 1;
            int smallest = right < Count && _data[right].CompareTo(_data[left]) < 0 
                ? right
                : left;

            if (_data[i].CompareTo(_data[smallest]) <= 0)
                return;

            Swap(i, smallest);            
            i = smallest;
        }
    }

    private readonly void Swap(int a, int b) 
        => (_data[a], _data[b]) = (_data[b], _data[a]);
}
