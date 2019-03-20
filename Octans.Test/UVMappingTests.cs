﻿using System;
using FluentAssertions;
using Xunit;

namespace Octans.Test
{
    public class UVMappingTests
    {
        [Fact]
        public void SphericalMap()
        {
            UVMapping.Spherical(new Point(0, 0, -1)).Should().Be((0.0f, 0.5f));
            UVMapping.Spherical(new Point(1, 0, 0)).Should().Be((0.25f, 0.5f));
            UVMapping.Spherical(new Point(0, 0, 1)).Should().Be((0.5f, 0.5f));
            UVMapping.Spherical(new Point(-1, 0, 0)).Should().Be((0.75f, 0.5f));
            UVMapping.Spherical(new Point(0, 1, 0)).Should().Be((0.5f, 1.0f));
            UVMapping.Spherical(new Point(0, -1, 0)).Should().Be((0.5f, 0.0f));
            var (u, v) = UVMapping.Spherical(new Point(MathF.Sqrt(2f) / 2f, MathF.Sqrt(2f) / 2f, 0));
            u.Should().BeApproximately(0.25f, 0.00001f);
            v.Should().BeApproximately(0.75f, 0.00001f);
        }

        [Fact]
        public void PlanarMap()
        {
            UVMapping.Planar(new Point(0.25f, 0, 0.5f)).Should().Be((0.25f, 0.5f));
            UVMapping.Planar(new Point(0.25f, 0, -0.25f)).Should().Be((0.25f, 0.75f));
            UVMapping.Planar(new Point(0.25f, 0.5f, -0.25f)).Should().Be((0.25f, 0.75f));
            UVMapping.Planar(new Point(1.25f, 0f, 0.5f)).Should().Be((0.25f, 0.5f));
            UVMapping.Planar(new Point(0.25f, 0f, -1.75f)).Should().Be((0.25f, 0.25f));
            UVMapping.Planar(new Point(1f, 0f, -1f)).Should().Be((0.0f, 0.0f));
            UVMapping.Planar(new Point(0f, 0f, 0f)).Should().Be((0.0f, 0.0f));
        }

        [Fact]
        public void CylindricalMap()
        {
            UVMapping.Cylindrical(new Point(0f, 0, -1f)).Should().Be((0.0f, 0.0f));
            UVMapping.Cylindrical(new Point(0f, 0.5f, -1f)).Should().Be((0.0f, 0.5f));
            UVMapping.Cylindrical(new Point(0f, 1f, -1f)).Should().Be((0.0f, 0.0f));
            UVMapping.Cylindrical(new Point(0.70711f, 0.5f, -0.70711f)).Should().Be((0.125f, 0.5f));
            UVMapping.Cylindrical(new Point(1f, 0.5f, 0f)).Should().Be((0.25f, 0.5f));
            UVMapping.Cylindrical(new Point(0.70711f, 0.5f, 0.70711f)).Should().Be((0.375f, 0.5f));
            UVMapping.Cylindrical(new Point(0.0f, -0.25f, 1f)).Should().Be((0.5f, 0.75f));
            UVMapping.Cylindrical(new Point(-0.70711f, 0.5f, 0.70711f)).Should().Be((0.625f, 0.5f));
            UVMapping.Cylindrical(new Point(-1f, 1.25f, 0f)).Should().Be((0.75f, 0.25f));
            UVMapping.Cylindrical(new Point(-0.70711f, 0.5f, -0.70711f)).Should().Be((0.875f, 0.5f));
        }
    }
}