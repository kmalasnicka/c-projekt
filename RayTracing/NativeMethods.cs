using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace RayTracing;

internal static partial class NativeMethods
{
    private const string LibName = "rt";

    [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
    internal delegate void RenderCallback(int samples, IntPtr buffer);

    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct CameraConfig{
        public double AspectRatio;
        public int ImageWidth;
        public int SamplesPerPixel;
        public int MaxDepth;
        public double Vfov;
        public fixed double LookFrom[3];
        public fixed double LookAt[3];
        public fixed double Vup[3];
        public double DefocusAngle;
        public double FocusDist;
    }

    [LibraryImport(LibName, EntryPoint = "CreateScene")]
    internal static partial IntPtr CreateScene_Native();

    [LibraryImport(LibName, EntryPoint = "DestroyScene")]
    internal static partial void DestroyScene(IntPtr scene);

    [LibraryImport(LibName, EntryPoint = "SceneClear")]
    internal static partial void SceneClear(SceneSafeHandle scene);

    [LibraryImport(LibName, EntryPoint = "SceneAddSphere")]
    internal static partial void SceneAddSphere(SceneSafeHandle scene, double cx, double cy, double cz, double radius, MaterialSafeHandle material);

    [LibraryImport(LibName, EntryPoint = "CreateLambertian")]
    internal static partial IntPtr CreateLambertian_Native(double r, double g, double b);

    [LibraryImport(LibName, EntryPoint = "CreateMetal")]
    internal static partial IntPtr CreateMetal_Native(double r, double g, double b, double fuzz);

    [LibraryImport(LibName, EntryPoint = "CreateDielectric")]
    internal static partial IntPtr CreateDielectric_Native(double refractionIndex);

    [LibraryImport(LibName, EntryPoint = "DestroyMaterial")]
    internal static partial void DestroyMaterial(IntPtr material);

    [LibraryImport(LibName, EntryPoint = "RenderScene")]
    internal static partial void RenderScene(SceneSafeHandle scene, CameraConfig config, IntPtr outRgba, RenderCallback callback);

    [LibraryImport(LibName, EntryPoint = "SavePng", StringMarshalling = StringMarshalling.Utf8)]
    internal static partial int SavePng(string filename, int width, int height, IntPtr rgba);

    internal static SceneSafeHandle CreateScene() => new SceneSafeHandle(CreateScene_Native());

    internal static MaterialSafeHandle CreateLambertian(double r, double g, double b) => new MaterialSafeHandle(CreateLambertian_Native(r, g, b));

    internal static MaterialSafeHandle CreateMetal(double r, double g, double b, double fuzz) => new MaterialSafeHandle(CreateMetal_Native(r, g, b, fuzz));

    internal static MaterialSafeHandle CreateDielectric(double refractionIndex) => new MaterialSafeHandle(CreateDielectric_Native(refractionIndex));
}

internal sealed class SceneSafeHandle : SafeHandle
{
    internal SceneSafeHandle(IntPtr ptr) : base(IntPtr.Zero, ownsHandle: true) => SetHandle(ptr);
    private SceneSafeHandle() : base(IntPtr.Zero, ownsHandle: true) { }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        NativeMethods.DestroyScene(handle);
        return true;
    }
}

internal sealed class MaterialSafeHandle : SafeHandle
{
    internal MaterialSafeHandle(IntPtr ptr) : base(IntPtr.Zero, ownsHandle: true) => SetHandle(ptr);
    private MaterialSafeHandle() : base(IntPtr.Zero, ownsHandle: true) { }

    public override bool IsInvalid => handle == IntPtr.Zero;

    protected override bool ReleaseHandle()
    {
        NativeMethods.DestroyMaterial(handle);
        return true;
    }
}