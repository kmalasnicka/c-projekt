using System;
namespace RayTracing;

public sealed class Scene : IDisposable{
    internal SceneSafeHandle Handle {get;}
    private bool _disposed;

    public Scene(){
        Handle = NativeMethods.CreateScene();
    }

    public void Clear(){
        if (_disposed) throw new ObjectDisposedException(nameof(Scene));
        NativeMethods.SceneClear(Handle);
    }

    public void AddSphere(double x, double y, double z, double radius, Material material){
        if(material == null) throw new ArgumentNullException(nameof(material));
        if (radius <= 0) throw new ArgumentOutOfRangeException(nameof(radius));
        if (_disposed) throw new ObjectDisposedException(nameof(Scene));

        NativeMethods.SceneAddSphere(Handle, x, y, z, radius, material.Handle);
    }

    public void Dispose(){
        if(_disposed) return;
        Handle.Dispose();
        _disposed = true;
    }
}