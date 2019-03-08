﻿using System;
using FluentAssertions;
using Xunit;

namespace Octans.Test
{
    public class ShadingTests
    {
        [Fact]
        public void EyeBetweenLightAndSurface()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, 0, -1);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1f, 1f, 1f));
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, false);
            result.Should().BeEquivalentTo(new Color(1.9f, 1.9f, 1.9f));
        }

        [Fact]
        public void EyeOffset45Deg()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, MathF.Sqrt(2) / 2, -MathF.Sqrt(2) / 2);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1f, 1f, 1f));
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, false);
            result.Should().BeEquivalentTo(new Color(1.0f, 1.0f, 1.0f));
        }

        [Fact]
        public void LightOffset45Deg()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, 0, -1f);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 10, -10), new Color(1f, 1f, 1f));
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, false);
            result.Should().BeEquivalentTo(new Color(0.7364f, 0.7364f, 0.7364f));
        }

        [Fact]
        public void FullReflectionOfLightOffset45Deg()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, -MathF.Sqrt(2) / 2, -MathF.Sqrt(2) / 2);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 10, -10), new Color(1f, 1f, 1f));
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, false);
            result.Should().BeEquivalentTo(new Color(1.6364f, 1.6364f, 1.6364f));
        }

        [Fact]
        public void LightBehindSurface()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, 0, -1f);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, 10), new Color(1f, 1f, 1f));
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, false);
            result.Should().BeEquivalentTo(new Color(0.1f, 0.1f, 0.1f));
        }

        [Fact]
        public void LightOnSurfaceInShadow()
        {
            var s = new Sphere();
            var m = new Material();
            var position = Point.Zero;
            var eyeV = new Vector(0, 0, -1f);
            var normalV = new Vector(0, 0, -1);
            var light = new PointLight(new Point(0, 0, -10), new Color(1f, 1f, 1f));
            const bool inShadow = true;
            var result = Shading.Lighting(m, s, light, position, eyeV, normalV, inShadow);
            result.Should().BeEquivalentTo(new Color(0.1f, 0.1f, 0.1f));
        }

        [Fact]
        public void ShadingOutsideIntersection()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var shape = w.Objects[0];
            var i = new Intersection(4f, shape);
            var comps = new IntersectionInfo(i, r);
            var c = Shading.HitColor(w, comps);
            c.Should().Be(new Color(0.38066f, 0.47583f, 0.2855f));
        }

        [Fact]
        public void ShadingInsideIntersection()
        {
            var w = World.Default();
            w.SetLights(new PointLight(new Point(0, 0.25f, 0), new Color(1f, 1f, 1f)));
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = w.Objects[1];
            var i = new Intersection(0.5f, shape);
            var comps = new IntersectionInfo(i, r);
            var c = Shading.HitColor(w, comps);
            c.Should().Be(new Color(0.90498f, 0.90498f, 0.90498f));
        }

        [Fact]
        public void ColorWhenRayMissesIsBlack()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 1, 0));
            var c = Shading.ColorAt(w, r);
            c.Should().Be(new Color(0, 0, 0));
        }

        [Fact]
        public void ColorWhenRayHits()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var c = Shading.ColorAt(w, r);
            c.Should().Be(new Color(0.38066f, 0.47583f, 0.2855f));
        }

        [Fact]
        public void ColorWithIntersectionBehind()
        {
            var w = World.Default();
            w.Objects[0].Material.Ambient = 1f;
            w.Objects[1].Material.Ambient = 1f;
            var r = new Ray(new Point(0, 0, 0.75f), new Vector(0, 0, -1));
            var c = Shading.ColorAt(w, r);
            c.Should().Be(w.Objects[1].Material.Pattern.LocalColorAt(Point.Zero));
        }

        [Fact]
        public void NotInShadowWhenNothingBetweenLightAndPoint()
        {
            var w = World.Default();
            var p = new Point(0, 10, 0);
            Shading.IsShadowed(w, p).Should().BeFalse();
        }

        [Fact]
        public void InShadowWhenAnObjectIsBetweenLightAndPoint()
        {
            var w = World.Default();
            var p = new Point(10, -10, 10);
            Shading.IsShadowed(w, p).Should().BeTrue();
        }

        [Fact]
        public void NotInShadowWhenObjectIsBehindPoint()
        {
            var w = World.Default();
            var p = new Point(-2, 2, -2);
            Shading.IsShadowed(w, p).Should().BeFalse();
        }

        [Fact]
        public void ReflectedColorForNonReflectiveMaterialIsBlack()
        {
            var w = World.Default();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var shape = w.Objects[1];
            shape.Material.Ambient = 1f;
            var i = new Intersection(1, shape);
            var comps = new IntersectionInfo(i, r);
            Shading.ReflectedColor(w, comps).Should().Be(Colors.Black);
        }

        [Fact]
        public void ReflectedColorForReflectiveMaterialIsNotBlack()
        {
            var w = World.Default();
            var shape = new Plane();
            shape.SetMaterial(new Material {Reflective = 0.5f});
            shape.SetTransform(Transforms.Translate(0, -1, 0));
            w.AddObject(shape);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f));
            var i = new Intersection(MathF.Sqrt(2f), shape);
            var comps = new IntersectionInfo(i, r);
            Shading.ReflectedColor(w, comps).Should().Be(new Color(0.19032f, 0.2379f, 0.14274f));
        }

        [Fact]
        public void HitColorIncludeReflectedColor()
        {
            var w = World.Default();
            var shape = new Plane();
            shape.SetMaterial(new Material { Reflective = 0.5f });
            shape.SetTransform(Transforms.Translate(0, -1, 0));
            w.AddObject(shape);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f));
            var i = new Intersection(MathF.Sqrt(2f), shape);
            var comps = new IntersectionInfo(i, r);
            Shading.HitColor(w, comps).Should().Be(new Color(0.87677f, 0.92436f, 0.82918f));
        }

        [Fact]
        public void ReflectedColorIsBlackIfRemainingBouncesIsZero()
        {
            var w = World.Default();
            var shape = new Plane();
            shape.SetMaterial(new Material { Reflective = 0.5f });
            shape.SetTransform(Transforms.Translate(0, -1, 0));
            w.AddObject(shape);
            var r = new Ray(new Point(0, 0, -3), new Vector(0, -MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f));
            var i = new Intersection(MathF.Sqrt(2f), shape);
            var comps = new IntersectionInfo(i, r);
            Shading.ReflectedColor(w, comps, 0).Should().Be(Colors.Black);
        }

        [Fact]
        public void HandleParallelReflectiveSurfaces()
        {
            var w = new World();
            w.SetLights(new PointLight(new Point(0, 0, 0), Colors.White));
            var lower = new Plane { Material = { Reflective = 1f } };
            lower.SetTransform(Transforms.Translate(0, -1, 0));

            var upper = new Plane { Material = { Reflective = 1f } };
            upper.SetTransform(Transforms.Translate(0, 1, 0));

            w.SetObjects(lower, upper);
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 1, 0));
            // Test to ensure an infinite loop is not hit.
            Shading.ColorAt(w, r).Should().NotBe(new Color(1, 0, 0));
        }

        [Fact]
        public void RefractedColorOfOpaqueSurfaceIsBlack()
        {
            var w = World.Default();
            var s = w.Objects[0];
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = new Intersections(new Intersection(4f, s), new Intersection(6f, s));
            var comps = new IntersectionInfo(xs[0], r, xs);
            Shading.RefractedColor(w, comps, 5).Should().Be(Colors.Black);
        }


        [Fact]
        public void RefractedColorIsBlackIfRemainingBouncesIsZero()
        {
            var w = World.Default();
            var s = w.Objects[0];
            s.Material.Transparency = 1.0f;
            s.Material.RefractiveIndex = 1.5f;
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = new Intersections(new Intersection(4f, s), new Intersection(6f, s));
            var comps = new IntersectionInfo(xs[0], r, xs);
            Shading.RefractedColor(w, comps, 0).Should().Be(Colors.Black);
        }

        [Fact]
        public void RefractedColorIsBlackForTotalInternalRefraction()
        {
            var w = World.Default();
            var s = w.Objects[0];
            s.Material.Transparency = 1.0f;
            s.Material.RefractiveIndex = 1.5f;
            var r = new Ray(new Point(0, 0, MathF.Sqrt(2f)/2f), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-MathF.Sqrt(2f) / 2f, s), 
                new Intersection(MathF.Sqrt(2f) / 2f, s));
            // Inside sphere so look at second intersection.
            var comps = new IntersectionInfo(xs[1], r, xs);
            Shading.RefractedColor(w, comps, 5).Should().Be(Colors.Black);
        }

        [Fact]
        public void RefractsRayWhenHitsTransparentSurface()
        {
            var w = World.Default();
            var a = w.Objects[0];
            a.Material.Ambient = 1.0f;
            a.Material.Pattern = new TestPattern();
            var b = w.Objects[1];
            b.Material.Transparency = 1.0f;
            b.Material.RefractiveIndex = 1.5f;
            var r = new Ray(new Point(0, 0, 0.1f), new Vector(0, 1, 0));
            var xs = new Intersections(
                new Intersection(-0.9899f, a),
                new Intersection(-0.4899f, b),
                new Intersection(0.4899f, b),
                new Intersection(0.9899f, a));
            var comps = new IntersectionInfo(xs[2], r, xs);
            Shading.RefractedColor(w, comps, 5).Should().Be(new Color(0, 0.99888f, 0.04725f));
        }

        [Fact]
        public void HitColorIncludeRefractedColor()
        {
            var w = World.Default();
            var floor = new Plane {Material = {Transparency = 0.5f, RefractiveIndex = 1.5f}};
            floor.SetTransform(Transforms.Translate(0, -1, 0));
            w.AddObject(floor);

            var ball = new Sphere {Material = {Pattern = new SolidColor(new Color(1, 0, 0)), Ambient = 0.5f}};
            ball.SetTransform(Transforms.Translate(0, -3.5f, -0.5f));
            w.AddObject(ball);

            var r = new Ray(new Point(0, 0, -3), new Vector(0, -MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f));
            var i = new Intersection(MathF.Sqrt(2f), floor);
            var xs = new Intersections(i);
            var comps = new IntersectionInfo(xs[0], r,xs);
            Shading.HitColor(w, comps, 5).Should().Be(new Color(0.93642f, 0.68642f, 0.68642f));
        }
    }
}