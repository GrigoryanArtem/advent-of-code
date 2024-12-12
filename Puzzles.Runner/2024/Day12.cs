namespace Puzzles.Runner._2024;

[Puzzle("Garden Groups", 12, 2024)]
public class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = char.MinValue;

    private const int NO_ID = -1;
    public const int TOP = 0;
    public const int LEFT = 3;

    private int[] _directions = [];

    private char[] _map = [];
    private int[] _ids = [];

    private int _idCount;

    private int _sizeX = 0;
    private Queue<int> _queue = [];

    public void Init()
    {
        InitMap();
        CalculateIds();
    }
   
    public string SolvePart1()
    {
        var p = new int[_idCount];
        var a = new int[_idCount];

        for (int i = _sizeX; i < _map.Length - _sizeX; i++)
        {
            if (_map[i] == BORDER)
                continue;

            var (id, val) = (_ids[i], _map[i]);
            a[id]++;

            for (int d = 0; d < _directions.Length; d++)
                if (id != _ids[i + _directions[d]])
                    p[id]++;
        }

        return p.Zip(a, (pv, av) => pv * av).Sum().ToString();
    }

    public string SolvePart2()
    {
        var p = new int[_idCount];
        var a = new int[_idCount];

        for (int i = _sizeX; i < _map.Length - _sizeX; i++)
        {
            if (_map[i] == BORDER)
                continue;

            var (id, val) = (_ids[i], _map[i]);            
            a[id]++;            
            
            for (int d = 0; d < _directions.Length; d++)
            {
                var d1 = _directions[d];
                var d2 = _directions[(d + 1) % _directions.Length];

                if (id != _ids[i + d1] && id != _ids[i + d2])
                    p[id]++;

                if (id == _ids[i + d1] && id == _ids[i + d2] && id != _ids[i + d1 + d2])
                    p[id]++;
            }
        }

        return p.Zip(a, (pv, av) => pv * av).Sum().ToString();
    }

    #region Private methods

    private void InitMap()
    {
        _sizeX = input.Lines.First().Length + 2;
        var sy = input.Lines.Length + 2;

        _map = new char[_sizeX * sy];

        for (int x = 0; x < _sizeX; x++)
            _map[Mat2Vec(x, 0)] = _map[Mat2Vec(x, sy - 1)] = BORDER;

        for (int y = 0; y < sy; y++)
            _map[Mat2Vec(0, y)] = _map[Mat2Vec(_sizeX - 1, y)] = BORDER;

        for (int y = 0; y < input.Lines.Length; y++)
            for (int x = 0; x < input.Lines[y].Length; x++)
                _map[Mat2Vec(x + 1, y + 1)] = input.Lines[y][x];

        _directions = [-sy, 1, sy, -1];
        _queue = new Queue<int>(_map.Length);
    }


    private void CalculateIds()
    {
        _ids = new int[_map.Length];
        Array.Fill(_ids, NO_ID);

        _idCount = 0;
        for (int i = _sizeX; i < _ids.Length - _sizeX; i++, _idCount++)
        {
            if (_map[i] == BORDER || _ids[i] != NO_ID)
                continue;

            _ids[i] = _idCount;

            _queue.Clear();
            _queue.Enqueue(i);

            while (_queue.Count > 0)
            {
                var current = _queue.Dequeue();

                Array.ForEach(_directions, d =>
                {
                    if (_map[current + d] != BORDER && _ids[current + d] == NO_ID && _map[current] == _map[current + d])
                    {
                        _ids[current + d] = _idCount;
                        _queue.Enqueue(current + d);
                    }
                });
            }
        }
    }

    private int Mat2Vec(int x, int y)
        => y * _sizeX + x;

    #endregion
}
