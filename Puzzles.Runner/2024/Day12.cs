namespace Puzzles.Runner._2024;

[Puzzle("Garden Groups", 12, 2024)]
public class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private delegate void RefFunc(int location, ref int accumulator);

    #region Constants 

    private const char BORDER = char.MinValue;
    private const int NO_ID = -1;

    #endregion

    #region Members

    private int _sizeX = 0;
    private char[] _map = [];
    private int[] _directions = [];

    private int _idCount;
    private int[] _ids = [];

    private Queue<int> _bfsQ = [];

    #endregion

    public void Init()
    {
        InitMap();
        CalculateIds();
    }

    public string SolvePart1()
        => CalculatePrice(Perimeter).ToString();

    public string SolvePart2()
        => CalculatePrice(Corners).ToString();

    #region Private methods

    private int CalculatePrice(RefFunc func)
    {
        var accumulator = new int[_idCount];
        var area = new int[_idCount];

        for (int i = _sizeX; i < _map.Length - _sizeX; i++)
        {
            if (_map[i] == BORDER)
                continue;

            area[_ids[i]]++;
            func(i, ref accumulator[_ids[i]]);
        }

        return accumulator.Zip(area, (pv, av) => pv * av).Sum();
    }

    private void Perimeter(int location, ref int p)
    {
        for (int d = 0; d < _directions.Length; d++)
            if (_ids[location] != _ids[location + _directions[d]])
                p++;
    }

    private void Corners(int location, ref int c)
    {
        var id = _ids[location];

        for (int d = 0; d < _directions.Length; d++)
        {
            var d1 = _directions[d];
            var d2 = _directions[(d + 1) % _directions.Length];

            if (id != _ids[location + d1] && id != _ids[location + d2])
                c++;

            if (id == _ids[location + d1] && id == _ids[location + d2] && id != _ids[location + d1 + d2])
                c++;
        }
    }

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
        _bfsQ = new Queue<int>(_map.Length);
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

            _bfsQ.Clear();
            _bfsQ.Enqueue(i);

            while (_bfsQ.Count > 0)
            {
                var current = _bfsQ.Dequeue();

                Array.ForEach(_directions, d =>
                {
                    if (_map[current + d] != BORDER && _ids[current + d] == NO_ID && _map[current] == _map[current + d])
                    {
                        _ids[current + d] = _idCount;
                        _bfsQ.Enqueue(current + d);
                    }
                });
            }
        }
    }

    private int Mat2Vec(int x, int y)
        => y * _sizeX + x;

    #endregion
}
