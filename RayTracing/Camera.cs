using System;
namespace RayTracing;

public sealed class Camera
{
    public double AspectRatio {get; set;}
    public int ImageWidth {get; set;}
    public int SamplesPerPixel {get; set;}
    public int MaxDepth {get; set;}
    public double Vfov {get; set;}
    public Vec3 LookFrom {get; set;}
    public Vec3 LookAt {get; set;}
    public Vec3 Vup {get; set;}
    public double DefocusAngle {get; set;}
    public double FocusDist {get; set;}
    public int ImageHeight => (int)(ImageWidth / AspectRatio);

    public void Validate()
    {
        if (AspectRatio <= 0) throw new ArgumentOutOfRangeException(nameof(AspectRatio));
        if (ImageWidth <= 0) throw new ArgumentOutOfRangeException(nameof(ImageWidth));
        if (SamplesPerPixel <= 0) throw new ArgumentOutOfRangeException(nameof(SamplesPerPixel));
        if (MaxDepth <= 0) throw new ArgumentOutOfRangeException(nameof(MaxDepth));
        if (Vfov <= 0) throw new ArgumentOutOfRangeException(nameof(Vfov));
        if (FocusDist <= 0) throw new ArgumentOutOfRangeException(nameof(FocusDist));
        if (ImageHeight <= 0) throw new InvalidOperationException("invalid ImageHeight");
    }
}
