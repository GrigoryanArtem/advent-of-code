using OpenCvSharp;

namespace Puzzles.Visuals.Model;
public class FileRenderer : IRenderer
{
    private VideoWriter _writer;

    public FileRenderer(int fps, Size frameSize)
    {
        FPS = fps;

        Frame = new Mat(frameSize, MatType.CV_8UC3); ;
        BaseFrame = new Mat(frameSize, MatType.CV_8UC3);

        var fourCC = FourCC.FromString("mp4v");
        _writer = new VideoWriter("visualization.mp4", fourCC, 60, frameSize);
    }

    public void Save()
        => _writer.Dispose();

    public int FPS { get; }

    public Mat Frame { get; set; } 
    public Mat BaseFrame { get; }

    public void Render()
    {
        _writer.Write(Frame);
        BaseFrame.CopyTo(Frame);
    }
}
