using System;
namespace RayTracing;

public abstract class Material : IDisposable
{
    internal MaterialSafeHandle Handle {get;}
    private bool _disposed;

    internal Material(MaterialSafeHandle handle){
        if (handle == null) throw new ArgumentNullException(nameof(handle));
        Handle = handle;
    }

    public void Dispose(){
        if (_disposed) return;
        Handle.Dispose();
        _disposed = true;
    }
}

public sealed class Lambertian : Material{
    public Lambertian(double r, double g, double b) : base(NativeMethods.CreateLambertian(r, g, b)) { }
}

public sealed class Metal : Material{
    public Metal(double r, double g, double b, double fuzz) : base(NativeMethods.CreateMetal(r, g, b, fuzz)) { }
}

public sealed class Dielectric : Material{
    public Dielectric(double refractionIndex) : base(NativeMethods.CreateDielectric(refractionIndex)) { }
}
