﻿using System;
using System.Diagnostics;
using System.IO;
using Octans.Camera;
using Octans.Geometry;
using Octans.IO;
using Octans.Light;
using Octans.Pipeline;
using Octans.Shading;
using Octans.Texture;

namespace Octans.ConsoleApp
{
    internal static partial class TestScenes
    {
        public static void MappingTestRender()
        {
            Cube Cube(float rotY, float rotX, float tx, float ty)
            {
                var cube1 = new Cube
                {
                    Material = {Texture = CreateTestCubeMap()}
                };
                cube1.SetTransform(Transforms.RotateY(rotY).RotateX(rotX).Translate(tx, ty, 0));
                return cube1;
            }

            var g = new Group();
            g.AddChild(Cube(0.7854f, 0.7854f, -6, 2));
            g.AddChild(Cube(2.3562f, 0.7854f, -2, 2));
            g.AddChild(Cube(3.927f, 0.7854f, 2, 2));
            g.AddChild(Cube(5.4978f, 0.7854f, 6, 2));
            g.AddChild(Cube(0.7854f, -0.7854f, -6, -2));
            g.AddChild(Cube(2.3562f, -0.7854f, -2, -2));
            g.AddChild(Cube(3.927f, -0.7854f, 2, -2));
            g.AddChild(Cube(5.4978f, -0.7854f, 6, -2));

            g.Divide(1);

            var w = new World();
            w.SetLights(new PointLight(new Point(0, 2, -100), Colors.White));
            w.SetObjects(g);

            var width = 800;
            var height = 400;
            var transform = Transforms.View(new Point(0, 0, -20f), new Point(0, 0, 0), new Vector(0, 1, 0));
            var c = new PinholeCamera(transform, 0.8f, width, height);
            var scene = new Scene(c, new PhongWorldShading(1, w));
            var aaa = new AdaptiveRenderer(3, 0.1f, scene);
            var canvas = new Canvas(width, height);

            Console.WriteLine("Rendering at {0}x{1}...", width, height);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext.Render(canvas, aaa);
            PPM.ToFile(canvas, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "mapping");
            stopwatch.Stop();
            Console.WriteLine("Done ({0})", stopwatch.Elapsed);
        }

        public static void SkyBoxMappingTestRender()
        {
            Console.WriteLine("Loading sky box file...");
            var skyBoxFile = Path.Combine(GetExecutionPath(), "skyboxsun.ppm");
            Console.WriteLine("Parsing sky box file...");
            var skyBoxCanvas = PPM.ParseFile(skyBoxFile);
            var skyBoxTexture = new UVImage(skyBoxCanvas);
            var skyBoxMap = new SkyBoxMap(skyBoxTexture);


            Cube Cube(float rotY, float rotX, float tx, float ty)
            {
                var cube1 = new Cube
                {
                    Material = { Texture = skyBoxMap }
                };
                cube1.SetTransform(Transforms.RotateY(rotY).RotateX(rotX).Translate(tx, ty, 0));
                return cube1;
            }

            var g = new Group();
            g.AddChild(Cube(0.7854f, 0.7854f, -6, 2));
            g.AddChild(Cube(2.3562f, 0.7854f, -2, 2));
            g.AddChild(Cube(3.927f, 0.7854f, 2, 2));
            g.AddChild(Cube(5.4978f, 0.7854f, 6, 2));
            g.AddChild(Cube(0.7854f, -0.7854f, -6, -2));
            g.AddChild(Cube(2.3562f, -0.7854f, -2, -2));
            g.AddChild(Cube(3.927f, -0.7854f, 2, -2));
            g.AddChild(Cube(5.4978f, -0.7854f, 6, -2));

            g.Divide(1);

            var w = new World();
            w.SetLights(new PointLight(new Point(0, 2, -100), Colors.White));
            w.SetObjects(g);

            var width = 800;
            var height = 400;
            var transform = Transforms.View(new Point(0, 0, -20f), new Point(0, 0, 0), new Vector(0, 1, 0));
            var c = new PinholeCamera(transform, 0.8f, width, height);
            var scene = new Scene(c, new PhongWorldShading(1, w));
            var aaa = new AdaptiveRenderer(3, 0.1f, scene);
            var canvas = new Canvas(width, height);

            Console.WriteLine("Rendering at {0}x{1}...", width, height);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext.Render(canvas, aaa);
            PPM.ToFile(canvas, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "mapping_sky_box");
            stopwatch.Stop();
            Console.WriteLine("Done ({0})", stopwatch.Elapsed);
        }

        public static void SphereMappingTestRender()
        {
            Console.WriteLine("Loading file...");
            var filePath = Path.Combine(GetExecutionPath(), "winter_river_1k.ppm");
            Console.WriteLine("Parsing file...");
            var textureCanvas = PPM.ParseFile(filePath);
            var image = new UVImage(textureCanvas);
            var map = new TextureMap(image, UVMapping.Spherical);


            Sphere Sphere(float rotY, float rotX, float tx, float ty)
            {
                var s = new Sphere()
                {
                    Material = { Texture = map }
                };
                s.SetTransform(Transforms.RotateY(rotY).RotateX(rotX).Translate(tx, ty, 0));
                return s;
            }

            var g = new Group();
            g.AddChild(Sphere(0.7854f, 0.7854f, -6, 2));
            g.AddChild(Sphere(2.3562f, 0.7854f, -2, 2));
            g.AddChild(Sphere(3.927f, 0.7854f, 2, 2));
            g.AddChild(Sphere(5.4978f, 0.7854f, 6, 2));
            g.AddChild(Sphere(0.7854f, -0.7854f, -6, -2));
            g.AddChild(Sphere(2.3562f, -0.7854f, -2, -2));
            g.AddChild(Sphere(3.927f, -0.7854f, 2, -2));
            g.AddChild(Sphere(5.4978f, -0.7854f, 6, -2));

            g.Divide(1);

            var w = new World();
            w.SetLights(new PointLight(new Point(0, 2, -100), Colors.White));
            w.SetObjects(g);

            var width = 800;
            var height = 400;
            var transform = Transforms.View(new Point(0, 0, -20f), new Point(0, 0, 0), new Vector(0, 1, 0));
            var c = new PinholeCamera(transform, 0.8f, width, height);
            var scene = new Scene(c, new PhongWorldShading(1, w));
            var aaa = new AdaptiveRenderer(3, 0.1f, scene);
            var canvas = new Canvas(width, height);

            Console.WriteLine("Rendering at {0}x{1}...", width, height);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext.Render(canvas, aaa);
            PPM.ToFile(canvas, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "mapping_spheres");
            stopwatch.Stop();
            Console.WriteLine("Done ({0})", stopwatch.Elapsed);
        }

        public static void InsideSphere()
        {
            Console.WriteLine("Loading file...");
            var filePath = Path.Combine(GetExecutionPath(), "winter_river_1k.ppm");
            Console.WriteLine("Parsing file...");
            var textureCanvas = PPM.ParseFile(filePath);
            var image = new UVImage(textureCanvas);
            var map = new TextureMap(image, UVMapping.Spherical);

            var skySphere = new Sphere()
            {
                Material = { Texture = map, Ambient = 1.2f, CastsShadows = false, Transparency = 1f}
            };

            skySphere.SetTransform(Transforms.RotateY(2.1f).Scale(1000f));

            var s = new Sphere();
            s.Material.Roughness = 0.1f;
            s.Material.Texture = SolidColor.Create(new Color(1f, 0.0f, 0.0f));
            s.Material.SpecularColor = new Color(0.2f,0.2f,0.2f);
            s.Material.Metallic = 0f;
            s.Material.Ambient = 0f;

            var g = new Group();
            g.AddChild(skySphere);
            g.AddChild(s);

            g.Divide(1);

            var w = new World();
            w.SetLights(new PointLight(new Point(100, 100, -100), new Color(1f,1f,1f)));
            w.SetObjects(g);

            var width = 800;
            var height = 400;
            var transform = Transforms.View(new Point(0, 0, -6f), new Point(0, 0, 0), new Vector(0, 1, 0));
            var c = new PinholeCamera(transform, 0.8f, width, height);
            var ws = new ComposableWorldShading(1, GGXNormalDistribution.Instance, SchlickBeckmanGeometricShadow.Instance, SchlickFresnelFunction.Instance, w);
            var scene = new Scene(c, ws);
            var aaa = new AdaptiveRenderer(3, 0.005f, scene);
            var canvas = new Canvas(width, height);

            Console.WriteLine("Rendering at {0}x{1}...", width, height);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext.Render(canvas, aaa);
            PPM.ToFile(canvas, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "sky_sphere");
            stopwatch.Stop();
            Console.WriteLine("Done ({0})", stopwatch.Elapsed);
        }

        public static void ColRowTestRender()
        {

            Console.WriteLine("Loading file...");
            //var filePath = Path.Combine(GetExecutionPath(), "indoor_env.ppm");
            var filePath = Path.Combine(GetExecutionPath(), "winter_river_1k.ppm");
            Console.WriteLine("Parsing file...");
            var textureCanvas = PPM.ParseFile(filePath);
            var image = new UVImage(textureCanvas);
            var map = new TextureMap(image, UVMapping.Spherical);

            var skySphere = new Sphere()
            {
                Material = { Texture = map, Ambient = 1.2f, CastsShadows = false, Transparency = 1f }
            };

            skySphere.SetTransform(Transforms.RotateY(3.4f).Scale(1000f));

            var g = new Group();
            var dx = 2.75f;
            var dz = 3.5f;
            var y = 1f;
            var nX = 10;
            var nZ = 1;
            var delta = 1f / (nX * nZ);
            int n = 0;
            bool metallic = true;
            for (var z = 0; z < nZ; z++)
            {
                for (var x = 0; x < nX; x++)
                {
                    var s = new Sphere();
                    s.SetTransform(Transforms.Scale(1.1f).Translate(x * dx, y, z * dz));
                    var color = x % 2 == 0 ? new Color(0.8f, 0.8f, 0.9f) : new Color(1f, 0.3f, 0.3f);
                    s.Material.Texture = SolidColor.Create(color);
                    s.Material.SpecularColor = metallic ? color : new Color(0.2f,0.2f,0.2f);
                    s.Material.Roughness =  MathFunction.Saturate(n * delta + 0.01f);
                    s.Material.Metallic = metallic ? 1f : 0f;
                    s.Material.Ambient = 0f;
                    s.Material.Reflective = 0.9f;
                    g.AddChild(s);
                    n++;
                }
            }

            var lightGray = new Color(0.9f, 0.9f, 0.9f);
            var darkGray = new Color(0.1f, 0.9f, 0.1f);
            var s1 = new StripeTexture(lightGray, darkGray);
            var s2 = new StripeTexture(lightGray, darkGray);
            s2.SetTransform(Transforms.RotateY(MathF.PI / 2));
            var pattern = new BlendedCompositeTexture(s1, s2);
            pattern.SetTransform(Transforms.Scale(1f / 30f));

            var text = new CheckerTexture(new Color(0.3f,0.7f,0.3f), new Color(0.13f, 0.13f, 0.13f));
            text.SetTransform(Transforms.Scale(1f / 16f));

            var floor = new Cube
            {
                Material =
                {
                    Texture = text,
                    SpecularColor = new Color(0.3f,0.3f,0.3f),
                    Metallic = 0f,
                    Roughness = 0.5f,
                    Ambient = 0.0f
                }
            };
            floor.SetTransform(Transforms.TranslateY(-1f).Scale(40f));

            var min = g.LocalBounds().Min;
            var max = g.LocalBounds().Max;
            var dir = max - min;
            var mid = min + (dir * 0.5f);

            var g2 = new Group(g, floor, skySphere);
            //var g2 = new Group(g);
            g2.Divide(1);

            var w = new World();
            w.SetLights(new PointLight(new Point(mid.X, 500, -500), Colors.White));
            w.SetObjects(g2);

            var width = 1200;
            var height = 140;
            var transform = Transforms.View(new Point(mid.X, 6f, -32f), mid, new Vector(0, 1, 0));
            var c = new PinholeCamera(transform, MathF.PI / 4f, width, height);
            var ws = new ComposableWorldShading(3, GGXNormalDistribution.Instance, GGXSmithGeometricShadow.Instance, SchlickFresnelFunction.Instance, w);
            //var ws = new PhongWorldShading(3, w);
            var scene = new Scene(c, ws);
            var aaa = new AdaptiveRenderer(3, 0.01f, scene);
            var canvas = new Canvas(width, height);

            Console.WriteLine("Rendering at {0}x{1}...", width, height);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            RenderContext.Render(canvas, aaa);
            PPM.ToFile(canvas, Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "col_row");
            stopwatch.Stop();
            Console.WriteLine("Done ({0})", stopwatch.Elapsed);
        }
    }
}