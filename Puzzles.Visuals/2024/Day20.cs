using OpenCvSharp;

namespace Puzzles.Visuals._2024;

using Map = Map2<char>;

public class Day20
{
    #region Constants

    private const int SCALE = 9;

    private const char START = 'S';
    private const char END = 'E';

    private const char BORDER = '@';
    private const char OBSTRUCTION = '#';
    private const char EMPTY = '.';

    #endregion

    private Map? _map;

    private Mat _defaultFrame;
    private Mat _frame;

    private VideoWriter _writer;

    public Day20(ILinesInputReader input)
    {
        _map = Map.WithBorders
        (
            data: [.. input.Lines.SelectMany(line => line)],
            columns: input.Lines.First().Length,
            borderValue: BORDER
        );

        var size = new Size(_map.Columns * SCALE, _map.Rows * SCALE);
        var fourCC = FourCC.FromString("mp4v");
        _writer = new VideoWriter("visualization.mp4", fourCC, 60, size);

        _frame = new Mat();
        _defaultFrame = new Mat(size, MatType.CV_8UC3);

        foreach(var (d, idx) in _map.WithIndex())
        {
            var (x, y) = _map.D1toD2(idx);            
            DrawCell(_defaultFrame, x, y, ColorByCellData(d));
        }

        _defaultFrame.CopyTo(_frame);        
    }
    public void Save()
        => _writer.Dispose();
    public void Run()
    {
        Solve(_map!, 20, 100);
    }

    #region Private methods

    private void Render()
    {
        //Cv2.ImShow("Day 20", _frame);
        //Cv2.WaitKey(33 / 2);

        //Cv2.WaitKey(33);

        _writer.Write(_frame);
        _defaultFrame.CopyTo(_frame);
    }

    private void Solve(Map map, int depth, int minDistance)
    {
        var bfs = new BFS<char>(map, [OBSTRUCTION, BORDER]);

        var start = Array.IndexOf(map.Data, START);
        var end = Array.IndexOf(map.Data, END);

        var te = bfs.Full(start, map.CreateBuffer<int>());
        var ts = bfs.Full(end, map.CreateBuffer<int>());

        var path = new List<int>();
        ReconstructPath(map, te, end, start, path);        
        path.Reverse();

        var max = te.Where(v => v < Int32.MaxValue / 2).Max();

        for(int i = 1; i < path.Count; i++)
        {
            var loc = path[i];

            var (cx, cy) = map.D1toD2(loc); 
            var (px, py) = map.D1toD2(path[i - 1]);


            Cv2.Line(_defaultFrame,
                new OpenCvSharp.Point(cx * SCALE + SCALE / 2, cy * SCALE + SCALE / 2),
                new OpenCvSharp.Point(px * SCALE + SCALE / 2, py * SCALE + SCALE / 2),
                GetRainbowColor(1.0 - (double)te[path[i]] / max), thickness: 2);

            DrawCell(_frame, cx, cy, Scalar.FromRgb(148, 0, 211));

            foreach(var jmp in GetCheatCells(map, loc, depth, []))
            {
                var diff = te[end] - (te[loc] + ts[jmp.loc] + jmp.dist);

                var (jx, jy) = map.D1toD2(jmp.loc);
                if (diff >= minDistance)
                {
                    //Cv2.Circle(_frame, jx * SCALE + SCALE / 2, jy * SCALE + SCALE / 2, 3, Scalar.FromRgb(77, 245, 99), thickness: -1);
                    DrawCell(_frame, jx, jy, Scalar.FromRgb(125, 210, 162));
                }
                //else if (diff > 0)
                //{
                //    DrawCell(_frame, jx, jy, Scalar.FromRgb(210, 208, 125));
                //}
            }

            Render();
        }

        //return map.WithIndex()
        //    .Where(c => c.item != BORDER && c.item != OBSTRUCTION)
        //    .AsParallel()
        //    .Sum(cell => GetCheatCells(map, cell.index, depth, [])
        //        .Select(jmp => te[end] - (te[cell.index] + ts[jmp.loc] + jmp.dist))
        //        .Count(dst => dst >= minDistance));
    }

    private static void ReconstructPath(Map map, int[] distances, int start, int end, List<int> path)
    {
        if (start == end)
            return;

        var next = map.Directions
            .Select(d => start + d)
            .Where(d => map[d] != OBSTRUCTION && map[d] != BORDER)
            .OrderBy(d => distances[d])
            .First();

        path.Add(next);
        ReconstructPath(map, distances, next, end, path);
    }

    private static IEnumerable<(int loc, int dist)> GetCheatCells(Map map, int loc, int depth, HashSet<int> visited)
    {
        var queue = new Queue<(int loc, int d)>();
        queue.Enqueue((loc, 0));

        while (queue.TryDequeue(out var data))
        {
            var (current, distance) = data;

            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);

            if ((map[current] == EMPTY || map[current] == END) && distance > 0)
                yield return (current, distance);

            if (distance >= depth)
                continue;

            Array.ForEach(map.Directions, d => queue.Enqueue((current + d, distance + 1)));
        }
    }

    private void InitFrame()
    {

    }

    private void DrawCell(Mat mat, int x, int y, Scalar color)
    {
        mat.Rectangle(new OpenCvSharp.Point(x, y) * SCALE, new OpenCvSharp.Point(x + 1, y + 1) * SCALE, color, thickness: -1);
    }

    private Scalar ColorByCellData(char data) => data switch
    {
        BORDER => Scalar.Black,
        START => Scalar.White,
        END => Scalar.White,
        EMPTY => Scalar.FromRgb(175, 190, 202),
        _ => Scalar.FromRgb(57, 84, 105)
    };

    public static Scalar Gradient(double value)
    {        
        int red = (int)(value * 255);         
        int green = (int)((1 - value) * 255);
        int blue = 0;                         

        return Scalar.FromRgb(red, green, blue);  
    }

    public static Scalar GetRainbowColor(double value)
    {        
        int segment = (int)(value * 6); 
        double segmentFraction = (value * 6) - segment; 
        double red = 0, green = 0, blue = 0;

        switch (segment)
        {
            case 0: 
                red = 255;
                green = 255 * segmentFraction;
                blue = 0;
                break;
            case 1: 
                red = 255;
                green = 255;
                blue = 0;
                break;
            case 2: 
                red = 255 * (1 - segmentFraction);
                green = 255;
                blue = 0;
                break;
            case 3: 
                red = 0;
                green = 255;
                blue = 255 * segmentFraction;
                break;
            case 4: 
                red = 0;
                green = 255 * (1 - segmentFraction);
                blue = 255;
                break;
            case 5: 
                red = 255 * segmentFraction;
                green = 0;
                blue = 255;
                break;
        }

        // Return the color as a Scalar in BGR format
        return new Scalar(blue, green, red);
    }

    #endregion

}
