using OpenCvSharp;

namespace Puzzles.Visuals.Model;
public class WindowRenderer(int fps, Size frameSize) : IRenderer
{
    public int FPS { get; } = fps;

    public Mat Frame { get; set;  } = new Mat(frameSize, MatType.CV_8UC3);
    public Mat BaseFrame { get; } = new Mat(frameSize, MatType.CV_8UC3);

    public void Render()
    {
        Cv2.ImShow("Day ?", Frame);
        Cv2.WaitKey(1000 / FPS);

        BaseFrame.CopyTo(Frame);
    }
}
