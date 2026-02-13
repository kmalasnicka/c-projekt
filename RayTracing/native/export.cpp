#include "rtweekend.h"
#include "hittable_list.h"
#include "material.h"
#include "sphere.h"
#include "color.h"
#include "camera.h"
#include <vector>
#include <cstring>
#include <memory>
#include <cstdint>

#define STB_IMAGE_WRITE_IMPLEMENTATION
#include "external/stb_image_write.h"

#if defined(_WIN32) || defined(_WIN64)
#define EXPORT __declspec(dllexport)
#else
#define EXPORT __attribute__((visibility("default")))
#endif

using RenderCallback = void (*)(int samples, uint8_t* buffer);
struct CameraConfig{
    double aspect_ratio;
    int32_t image_width;
    int32_t samples_per_pixel;
    int32_t max_depth;
    double vfov;
    double lookfrom[3];
    double lookat[3];
    double vup[3];
    double defocus_angle;
    double focus_dist;
};

using SceneHandle = hittable_list;
using MaterialHandle = shared_ptr<material>;

extern "C" EXPORT SceneHandle* CreateScene();
extern "C" EXPORT void DestroyScene(SceneHandle* scene);
extern "C" EXPORT void SceneClear(SceneHandle* scene);
extern "C" EXPORT void SceneAddSphere(SceneHandle* scene, double cx, double cy, double cz, double radius, MaterialHandle* material);
extern "C" EXPORT MaterialHandle* CreateLambertian(double r, double g, double b);
extern "C" EXPORT MaterialHandle* CreateMetal(double r, double g, double b, double fuzz);
extern "C" EXPORT MaterialHandle* CreateDielectric(double refraction_index);
extern "C" EXPORT void DestroyMaterial(MaterialHandle* material);
extern "C" EXPORT void RenderScene(SceneHandle* scene, CameraConfig config, uint8_t* out_rgba, RenderCallback callback);
extern "C" EXPORT int SavePng(const char* filename, int32_t width, int32_t height, const uint8_t* rgba);


SceneHandle* CreateScene(){
    return new SceneHandle();
}

void DestroyScene(SceneHandle* scene){
    delete scene;
}

void SceneClear(SceneHandle* scene){
    if(!scene) return;
    scene->clear();
}

void SceneAddSphere(SceneHandle* scene, double cx, double cy, double cz, double radius, MaterialHandle* material){
    if(!scene || !material) return;
    scene->add(make_shared<sphere>(point3(cx, cy, cz), radius, *material));
}

MaterialHandle* CreateLambertian(double r, double g, double b){
    return new MaterialHandle(make_shared<lambertian>(color(r, g, b)));
}

MaterialHandle* CreateMetal(double r, double g, double b, double fuzz){
    return new MaterialHandle(make_shared<metal>(color(r, g, b), fuzz));
}

MaterialHandle* CreateDielectric(double refraction_index){
    return new MaterialHandle(make_shared<dielectric>(refraction_index));
}

void DestroyMaterial(MaterialHandle* material){
    delete material;
}

void RenderScene(SceneHandle* scene, CameraConfig config, uint8_t* out_rgba, RenderCallback callback){
    if(!scene || !out_rgba) return;
    camera cam;
    cam.aspect_ratio = config.aspect_ratio;
    cam.image_width = config.image_width;
    cam.samples_per_pixel = config.samples_per_pixel;
    cam.max_depth = config.max_depth;
    cam.vfov = config.vfov;
    cam.lookfrom = point3(config.lookfrom[0], config.lookfrom[1], config.lookfrom[2]);
    cam.lookat = point3(config.lookat[0], config.lookat[1], config.lookat[2]);
    cam.vup = vec3(config.vup[0], config.vup[1], config.vup[2]);
    cam.defocus_angle = config.defocus_angle;
    cam.focus_dist = config.focus_dist;
    cam.render(*scene, out_rgba, callback);
}

int SavePng(const char* filename, int32_t width, int32_t height, const uint8_t* rgba) {
    if (!filename || !rgba || width <= 0 || height <= 0) return 0;
    const int stride_bytes = width * 4;
    return stbi_write_png(filename, width, height, 4, rgba, stride_bytes);
}