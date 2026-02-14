using System;
using System.Diagnostics;
using RayTracing;
using Windowing;
namespace RayTracingDemo;

class Program
{
    static void Main(string[] args)
    {
        int imageWidth = 800;
        double aspect = 3.0 / 2.0;
        int imageHeight = (int)(imageWidth / aspect);
        
        Viewer.Show(imageWidth, imageHeight, "RayTracing Demo", updater =>
        {
            using Scene scene = new Scene();
            using Lambertian ground = new Lambertian(0.5, 0.5, 0.5);
            using Dielectric glass = new Dielectric(1.5);
            using Lambertian center = new Lambertian(0.4, 0.2, 0.1);
            using Metal right = new Metal(0.7, 0.6, 0.5, 0.0);
            
            scene.AddSphere(0, -1000, 0, 1000, ground);
            scene.AddSphere(0, 1, 0, 1.0, glass);
            scene.AddSphere(-4, 1, 0, 1.0, center);
            scene.AddSphere(4, 1, 0, 1.0, right);

            Camera cam = new Camera{
                AspectRatio = aspect,
                ImageWidth = imageWidth,
                SamplesPerPixel = 100, 
                MaxDepth = 50,
                Vfov = 20,
                LookFrom = new Vec3(13, 2, 3),
                LookAt = new Vec3(0, 0, 0),
                Vup = new Vec3(0, 1, 0),
                DefocusAngle = 0.6,
                FocusDist = 10.0
            };

            Stopwatch stopwatch = Stopwatch.StartNew();
            int lastShownSamples = 0;
            
            byte[] image = Renderer.Render(scene, cam, (samples, rgba) =>
            {
                if(updater.IsClosed) return;
                if(samples == lastShownSamples) return;
                if(samples - lastShownSamples < 2) return;
                lastShownSamples = samples;
                updater.UpdateImage(rgba);
                updater.UpdateStatus($"samples: {samples}/{cam.SamplesPerPixel} \ntime: {stopwatch.Elapsed.TotalSeconds:0.0}s");
            });

            Renderer.SavePng("output.png", imageWidth, imageHeight, image);
            updater.UpdateStatus($"output.png saved  \ntotal: {stopwatch.Elapsed.TotalSeconds:0.0}s");
        });
    }
}
       