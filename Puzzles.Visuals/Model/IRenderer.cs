using OpenCvSharp;

namespace Puzzles.Visuals.Model;

public interface IRenderer
{
    public Mat BaseFrame { get; }
    public Mat Frame { get; }

    public void Render();
}
