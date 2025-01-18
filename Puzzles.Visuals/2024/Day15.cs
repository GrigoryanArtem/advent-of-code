using OpenCvSharp;
using Puzzles.Base.Entities;
using Puzzles.Visuals.Model;

namespace Puzzles.Visuals._2024;

using Map = Map2<char>;

public class Day15
{
    #region Constants

    private const int SCALE_X = 10;
    private const int SCALE_Y = 20;

    private const char BORDER = '#';
    private const char ROBOT = '@';
    private const char BOX = 'O';
    private const char EMPTY = '.';

    private const char BOXL = '[';
    private const char BOXR = ']';

    #endregion

    private readonly FileRenderer _renderer;

    private Map? _map;
    private Map? _wideMap;

    private int[] _path = [];

    public Day15(ILinesInputReader input)
    {
        InitMap(input);
        InitWideMap();

        _renderer =  new(60, new Size(_wideMap.Columns * SCALE_X, _wideMap.Rows * SCALE_Y));

        foreach (var (d, idx) in _wideMap.WithIndex())
        {
            var (x, y) = _wideMap.D1toD2(idx);
            
            DrawCell(_renderer.BaseFrame, x, y, d == BORDER ? ColorByCellData(d) : Scalar.FromRgb(57, 84, 105));
        }
    }

    #region Private methods

    public void Run()
    {
        SumOfGPS(_wideMap, BOXL);
    }

    public void Save()
    {
        _renderer.Save();
    }

    private int SumOfGPS(Map map, char target)
    {
        List<int> path = [];
        var currentLocation = Array.IndexOf(map.Data, ROBOT);
        _path.WithIndex().ForEach(d => 
        {
            currentLocation = TryMove(map, currentLocation, d.item);

            if(d.index % 3 == 0)
                RenderMap(path);

            path.Add(currentLocation);
        });

        return map.WithIndex()
            .Where(c => c.item == target)
            .Sum(c => Distance(map, c.index));
    }

    private static int TryMove(Map map, int location, int ddx)
    {
        var next = map.Next(location, ddx);
        if (map[location] == BORDER || !CanMove(map, location, ddx, []))
        {
            return location;
        }
        else
        {
            Move(map, location, ddx, []);
            return next;
        }
    }

    private static bool CanMove(Map map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return true;

        visited.Add(location);
        var next = map.Next(location, ddx);
        return map[location] switch
        {
            BORDER => false,
            EMPTY => true,
            BOXL => CanMove(map, next, ddx, visited) && CanMove(map, map.Next(location, Map.RIGHT), ddx, visited),
            BOXR => CanMove(map, next, ddx, visited) && CanMove(map, map.Next(location, Map.LEFT), ddx, visited),
            _ => CanMove(map, next, ddx, visited)
        };
    }

    private static void Move(Map map, int location, int ddx, HashSet<int> visited)
    {
        if (visited.Contains(location))
            return;

        visited.Add(location);
        var next = map.Next(location, ddx);
        switch (map[location])
        {
            case BOX:
            case ROBOT:
                Move(map, next, ddx, visited);
                break;
            case BOXL:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map.RIGHT), ddx, visited);
                break;
            case BOXR:
                Move(map, next, ddx, visited);
                Move(map, map.Next(location, Map.LEFT), ddx, visited);
                break;
            default:
                return;
        }

        (map[next], map[location]) = (map[location], map[next]);
    }

    private void InitMap(ILinesInputReader input)
    {
        var sx = input.Lines.First().Length;
        _map = new
        (
            data: [..input.Lines
                .TakeWhile(line => line.Length > 0 && line.First() == BORDER)
                .SelectMany(line => line)],
            columns: sx
        );

        _path = input.Lines.Skip(_map.Rows)
            .SkipWhile(line => line.Length == 0)
            .SelectMany(line => line.Select(Ch2D))
            .ToArray();
    }

    private void InitWideMap()
        => _wideMap = new(_map!.SelectMany(WideChar).ToArray(), 2 * _map!.Columns);

    private static IEnumerable<char> WideChar(char symbol) => symbol switch
    {
        BORDER => [BORDER, BORDER],
        BOX => [BOXL, BOXR],
        EMPTY => [EMPTY, EMPTY],
        ROBOT => [ROBOT, EMPTY],
        _ => throw new NotImplementedException()
    };

    private static int Ch2D(char symbol) => symbol switch
    {
        '^' => 0,
        '>' => 1,
        'v' => 2,
        '<' => 3,
        _ => throw new NotImplementedException()
    };

    private static int Distance(Map map, int location)
    {
        var (x, y) = map.D1toD2(location);
        return y * 100 + x;
    }

    private void RenderMap(List<int> path)
    {
        for (int i = 1; i < path.Count; i++)
        {
            var (cx, cy) = _wideMap.D1toD2(path[i]);
            var (px, py) = _wideMap.D1toD2(path[i - 1]);

            var color = GetGradientColor(Scalar.FromRgb(57, 84, 105), GetRainbowColor((double)i / _path.Length), (0.15 + (1 - (Math.Min(1, (path.Count - i) / Math.Min(path.Count, 300.0)))) * 0.4));


            Cv2.Line(_renderer.Frame,
                new OpenCvSharp.Point(cx * SCALE_X + SCALE_X / 2, cy * SCALE_Y + SCALE_Y / 2),
                new OpenCvSharp.Point(px * SCALE_X + SCALE_X / 2, py * SCALE_Y + SCALE_Y / 2),
                color, thickness: 2);
        }

        foreach (var (d, idx) in _wideMap.WithIndex())
        {
            var (x, y) = _wideMap.D1toD2(idx);


            if (d == ROBOT)
            {
                DrawCell(_renderer.Frame, x, y, ColorByCellData(d));
                DrawBorder(_renderer.Frame, x, y);
            }

            if(d == BOXL)
            {
                DrawBorder(_renderer.Frame, x, y);
                _renderer.Frame.Rectangle(new OpenCvSharp.Point(x * SCALE_X + 1, y * SCALE_Y + 1), new OpenCvSharp.Point((x + 1) * SCALE_X, (y + 1) * SCALE_Y - 2), Scalar.RosyBrown, thickness: -1);
            }

            if (d == BOXR)
            {
                DrawBorder(_renderer.Frame, x, y);
                _renderer.Frame.Rectangle(new OpenCvSharp.Point(x * SCALE_X - 1, y * SCALE_Y + 1), new OpenCvSharp.Point((x + 1) * SCALE_X - 2, (y + 1) * SCALE_Y - 2), Scalar.RosyBrown, thickness: -1);
            }
        }

        double[] distortionCoeffs = { 0, 0.025, 0, 0, 0 };

        Mat noise = new Mat(_renderer.Frame.Size(), _renderer.Frame.Type());
        Cv2.Randn(noise, new Scalar(20, 20, 20), new Scalar(10, 10, 10));

        _renderer.Frame = ApplyLensDistortion(_renderer.Frame + noise, distortionCoeffs);
        _renderer.Render();
    }

    private void DrawCell(Mat mat, int x, int y, Scalar color)
    {
        mat.Rectangle(new OpenCvSharp.Point(x * SCALE_X, y * SCALE_Y), new OpenCvSharp.Point((x + 1) * SCALE_X - 1, (y + 1) * SCALE_Y - 1), color, thickness: -1);
    }

    private void DrawBorder(Mat mat, int x, int y)
    {
        mat.Rectangle(new OpenCvSharp.Point(x * SCALE_X, y * SCALE_Y), new OpenCvSharp.Point((x + 1) * SCALE_X - 1, (y + 1) * SCALE_Y - 1), Scalar.FromRgb(55, 55, 55), lineType: LineTypes.AntiAlias, thickness: 1);
    }

    private Scalar ColorByCellData(char data) => data switch
    {        
        ROBOT => Scalar.Magenta,

        BOXL => Scalar.RosyBrown,
        BOXR => Scalar.RosyBrown,

        BORDER => Scalar.FromRgb(175, 190, 202),
        _ => Scalar.FromRgb(57, 84, 105)
    };

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

        return new Scalar(blue, green, red);
    }

    public static Scalar GetGradientColor(Scalar startColor, Scalar endColor, double value)
    {
        value = Math.Clamp(value, 0.0, 1.0);

        double blue = startColor.Val0 + (endColor.Val0 - startColor.Val0) * value;
        double green = startColor.Val1 + (endColor.Val1 - startColor.Val1) * value;
        double red = startColor.Val2 + (endColor.Val2 - startColor.Val2) * value;

        return new Scalar(blue, green, red);
    }

    public static Mat ApplyLensDistortion(Mat input, double[] distortionCoeffs)
    {
        if (input == null || input.Empty())
        {
            throw new ArgumentNullException(nameof(input), "Input image cannot be null or empty.");
        }

        if (distortionCoeffs == null || distortionCoeffs.Length != 5)
        {
            throw new ArgumentException("Distortion coefficients must be an array of 5 elements.", nameof(distortionCoeffs));
        }


        double focalLength = input.Width / 2;
        Point2d principalPoint = new Point2d(input.Width / 2.0, input.Height / 2.0); 

        Mat cameraMatrix = new Mat(3, 3, MatType.CV_64F);
        cameraMatrix.Set<double>(0, 0, focalLength); // fx
        cameraMatrix.Set<double>(1, 1, focalLength); // fy
        cameraMatrix.Set<double>(0, 2, principalPoint.X); // cx
        cameraMatrix.Set<double>(1, 2, principalPoint.Y); // cy
        cameraMatrix.Set<double>(2, 2, 1.0); // 1

        Mat distCoeffs = Mat.FromArray(new double[,] {
            { distortionCoeffs[0], distortionCoeffs[1], distortionCoeffs[2], distortionCoeffs[3], distortionCoeffs[4] }
        });

        Mat map1 = new Mat();
        Mat map2 = new Mat();

        Cv2.InitUndistortRectifyMap(
            cameraMatrix, distCoeffs, new Mat(), cameraMatrix,
            input.Size(), MatType.CV_32FC1, map1, map2);

        Mat output = new Mat();
        Cv2.Remap(input, output, map1, map2, InterpolationFlags.Linear, BorderTypes.Constant);

        return output;
    }

    public static Mat AddStaticNoise(Mat input, int intensity)
    {
        if (input == null || input.Empty())
        {
            throw new ArgumentNullException(nameof(input), "Input image cannot be null or empty.");
        }

        if (intensity < 0 || intensity > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(intensity), "Intensity must be between 0 and 100.");
        }

        Mat noise = new Mat(input.Size(), input.Type());

        var rng = new Random();
        noise.SetTo(new Scalar(
            rng.Next(0, 256),
            rng.Next(0, 256), 
            rng.Next(0, 256)  
        ));

        Mat output = new Mat();
        double alpha = intensity / 100.0;
        double beta = 1.0 - alpha;       
        Cv2.AddWeighted(input, beta, noise, alpha, 0, output);

        return output;
    }

    #endregion
}
