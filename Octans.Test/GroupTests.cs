﻿using System;
using FluentAssertions;
using Xunit;

namespace Octans.Test
{
    public class GroupTests
    {
        [Fact]
        public void HasIdentityMatrixByDefault()
        {
            var g = new Group();
            g.Transform.Should().Be(Matrix.Identity);
        }

        [Fact]
        public void IsEmptyByDefault()
        {
            var g = new Group();
            g.Children.Should().BeEmpty();
        }

        [Fact]
        public void AddingChildToGroupAssignsGroupAsParent()
        {
            var g = new Group();
            var s = new TestShape();
            g.AddChild(s);
            s.Parent.Should().Be(g);
            g.Children.Should().OnlyContain(c=>c==s);
        }

        [Fact]
        public void NotIntersectionsWithEmptyGroup()
        {
            var g = new Group();
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var xs = g.LocalIntersects(r);
            xs.Should().BeEmpty();
        }

        [Fact]
        public void IntersectionsWithNonEmptyGroup()
        {
            var g = new Group();
            var s1 = new Sphere();
            var s2 = new Sphere();
            var s3 = new Sphere();
            s2.SetTransform(Transforms.TranslateZ(-3f));
            s3.SetTransform(Transforms.TranslateX(5f));
            g.AddChild(s1);
            g.AddChild(s2);
            g.AddChild(s3);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = g.LocalIntersects(r).ToSorted();
            xs.Should().HaveCount(4);
            xs[0].Shape.Should().Be(s2);
            xs[1].Shape.Should().Be(s2);
            xs[2].Shape.Should().Be(s1);
            xs[3].Shape.Should().Be(s1);
        }

        [Fact]
        public void CanTranslateGroup()
        {
            var g = new Group();
            g.SetTransform(Transforms.Scale(2f));
            var s = new Sphere();
            s.SetTransform(Transforms.TranslateX(5f));
            g.AddChild(s);
            var r = new Ray(new Point(10, 0, -10), new Vector(0, 0, 1));
            var xs = g.Intersects(r);
            xs.Should().HaveCount(2);
        }

        [Fact]
        public void LocalBoundsConsidersTransformOfChild()
        {
            var g = new Group();
            var c = new Cube();
            c.SetTransform(Transforms.RotateZ(MathF.PI / 4));
            g.AddChild(c);
            var b = g.LocalBounds();
            b.Min.Should().Be(new Point(-1/MathF.Sin(MathF.PI / 4), -1/MathF.Sin(MathF.PI / 4), -1));
            b.Max.Should().Be(new Point(1/MathF.Sin(MathF.PI / 4), 1/MathF.Sin(MathF.PI / 4), 1));
        }

        [Fact]
        public void LocalBoundsConsidersAllChildren()
        {
            var s = new Sphere();
            s.SetTransform(Transforms.Translate(2,5,-3) * Transforms.Scale(2f));
            var c = new Cylinder {Minimum = -2, Maximum = 2};
            c.SetTransform(Transforms.Translate(-4,-1,4) * Transforms.Scale(0.5f,1,0.5f));
            var g = new Group();
            g.AddChild(s);
            g.AddChild(c);
            var b = g.LocalBounds();
            b.Min.Should().Be(new Point(-4.5f, -3, -5));
            b.Max.Should().Be(new Point(4f, 7, 4.5f));
        }

        [Fact]
        public void ReturnsEmptyIntersectionsIfRayMissesBoundingBox()
        {
            var g = new Group();
            var c = new Cube();
            c.SetTransform(Transforms.TranslateX(10));
            g.AddChild(c);
            var r = new Ray(new Point(0, 0, 0), new Vector(0, 0, 1));
            var intersects = g.Intersects(r);
            intersects.Should().HaveCount(0);
        }

        [Fact]
        public void IntersectDoesNotTestChildrenIfRayMissesBounds()
        {
            var c = new TestShape();
            var g = new Group();
            g.AddChild(c);
            var r = new Ray(new Point(0,0,-5), new Vector(0,1,0) );
            var xs = g.Intersects(r);
            // Child not tested so SavedRay remains default.
            c.SavedRay.Should().Be(new Ray());
        }

        [Fact]
        public void IntersectTestChildrenIfRayHitsBounds()
        {
            var c = new TestShape();
            var g = new Group();
            g.AddChild(c);
            var r = new Ray(new Point(0, 0, -5), new Vector(0, 0, 1));
            var xs = g.Intersects(r);
            // Child tested so SavedRay in not default.
            c.SavedRay.Should().NotBe(new Ray());
        }
    }
}