using System;
using System.Runtime.InteropServices;
namespace RayTracing;

public static class Renderer{
    public static byte[] Render(Scene scene, Camera camera, Action<int, byte[]>? onProgress = null){
        if (scene == null) throw new ArgumentNullException(nameof(scene));
        if (camera == null) throw new ArgumentNullException(nameof(camera));
        camera.Validate();

        int width = camera.ImageWidth;
        int height = camera.ImageHeight;
        byte[] rgba = new byte[width * height * 4];

        GCHandle handle = GCHandle.Alloc(rgba, GCHandleType.Pinned);
        try{
            IntPtr outPtr = handle.AddrOfPinnedObject();
            NativeMethods.CameraConfig cfg = ToNative(camera);

            NativeMethods.RenderCallback callback = (samples, buffer) => {
                if (onProgress != null)
                    onProgress(samples, rgba);
            };

            NativeMethods.RenderScene(scene.Handle, cfg, outPtr, callback);
            return rgba;
        }
        finally{
            handle.Free();
        }
    }
    public static void SavePng(string filename, int width, int height, byte[] rgba){
        if (filename == null) throw new ArgumentNullException(nameof(filename));
        if (rgba == null) throw new ArgumentNullException(nameof(rgba));

        GCHandle handle = GCHandle.Alloc(rgba, GCHandleType.Pinned);
        try
        {
            NativeMethods.SavePng(filename, width, height, handle.AddrOfPinnedObject());
        }
        finally
        {
            handle.Free();
        }    
    }
    private static unsafe NativeMethods.CameraConfig ToNative(Camera c)
    {
        NativeMethods.CameraConfig cfg = new NativeMethods.CameraConfig();

        cfg.AspectRatio = c.AspectRatio;
        cfg.ImageWidth = c.ImageWidth;
        cfg.SamplesPerPixel = c.SamplesPerPixel;
        cfg.MaxDepth = c.MaxDepth;
        cfg.Vfov = c.Vfov;
        cfg.DefocusAngle = c.DefocusAngle;
        cfg.FocusDist = c.FocusDist;

        cfg.LookFrom[0] = c.LookFrom.X;
        cfg.LookFrom[1] = c.LookFrom.Y;
        cfg.LookFrom[2] = c.LookFrom.Z;

        cfg.LookAt[0] = c.LookAt.X;
        cfg.LookAt[1] = c.LookAt.Y;
        cfg.LookAt[2] = c.LookAt.Z;

        cfg.Vup[0] = c.Vup.X;
        cfg.Vup[1] = c.Vup.Y;
        cfg.Vup[2] = c.Vup.Z;

        return cfg;
    }
}