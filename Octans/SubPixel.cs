﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Octans
{
    internal class ScopedPixelSamples : IPixelSamples
    {
        private readonly IPixelSamples _shared;
        private readonly Dictionary<SubPixel, Color> _scoped = new Dictionary<SubPixel, Color>();

        public ScopedPixelSamples(IPixelSamples shared)
        {
            _shared = shared;
        }

        public Color GetOrAdd(in SubPixel sp, IPixelRenderer renderer)
        {
            if (_scoped.TryGetValue(sp, out var color))
            {
                return color;
            }

            if (sp.Dx == 0 || sp.Dy == 0)
            {
                return _shared.GetOrAdd(in sp, renderer);
            }

            color = renderer.Render(in sp);
            _scoped.Add(sp, color);
            return color;
        }

        public void Reset()
        {
            _scoped.Clear();
        }
    }

    internal class PixelSamples : ISharedPixelSamples
    {
        private readonly ConcurrentDictionary<SubPixel, Color> _shared = new ConcurrentDictionary<SubPixel, Color>();
        private readonly ObjectPool<IPixelSamples> _localPool;

        public PixelSamples()
        {
            _localPool = new ObjectPool<IPixelSamples>(LocalScopeFactory);
        }

        public Color GetOrAdd(in SubPixel sp, IPixelRenderer renderer)
        {
            return _shared.GetOrAdd(sp, p => renderer.Render(in p));
        }

        public void Reset()
        {
            // Does nothing
        }

        public IPixelSamples CreateLocalScope()
        {
            return _localPool.GetObject();
        }

        private ScopedPixelSamples LocalScopeFactory()
        {
            return new ScopedPixelSamples(this);
        }

        public void CloseLocalScope(IPixelSamples samples)
        {
            samples.Reset();
            _localPool.PutObject(samples);
        }
    }

    public interface ISharedPixelSamples : IPixelSamples
    {
        IPixelSamples CreateLocalScope();
        void CloseLocalScope(IPixelSamples samples);
    }

    public interface IPixelSamples
    {
        Color GetOrAdd(in SubPixel sp, IPixelRenderer renderer);
        void Reset();
    }

    public readonly struct SubPixel : IEquatable<SubPixel>
    {
        public int X { get; }
        public int Y { get; }
        public int Divisions { get; }
        public int Dx { get; }
        public int Dy { get; }

        public SubPixel(int x, int y, int divisions, int dx, int dy)
        {
            X = x;
            Y = y;
            Divisions = divisions;
            Dx = dx;
            Dy = dy;
        }

        public bool Equals(SubPixel other) => X == other.X && Y == other.Y && Divisions == other.Divisions &&
                                              Dx == other.Dx && Dy == other.Dy;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            return obj is SubPixel other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X;
                hashCode = (hashCode * 397) ^ Y;
                hashCode = (hashCode * 397) ^ Divisions.GetHashCode();
                hashCode = (hashCode * 397) ^ Dx.GetHashCode();
                hashCode = (hashCode * 397) ^ Dy.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SubPixel left, SubPixel right) => left.Equals(right);

        public static bool operator !=(SubPixel left, SubPixel right) => !left.Equals(right);

        public static (SubPixel tl, SubPixel tr, SubPixel bl, SubPixel br) Corners(in SubPixel center)
        {
            var x = center.X;
            var y = center.Y;
            var div = center.Divisions;
            var dx = center.Dx;
            var dy = center.Dy;
            var tl = Create(x, y, div, dx - 1, dy - 1);
            var tr = Create(x, y, div, dx + 1, dy - 1);
            var bl = Create(x, y, div, dx - 1, dy + 1);
            var br = Create(x, y, div, dx + 1, dy + 1);
            return (tl, tr, bl, br);
        }

        public static SubPixel Create(int x, int y, int divisions, int dx, int dy)
        {
            if (dx == divisions)
            {
                x += 1;
                dx = 0;
            }

            if (dy == divisions)
            {
                y += 1;
                dy = 0;
            }

            while (divisions > 1 && dx % 2 == 0 && dy % 2 == 0)
            {
                dx >>= 1;
                dy >>= 1;
                divisions >>= 1;
            }

            return new SubPixel(x, y, divisions, dx, dy);
        }

        private static SubPixel ToDivResolution(in SubPixel sp, int resolution)
        {
            if (sp.Divisions == resolution)
            {
                return sp;
            }

            var fac = resolution / sp.Divisions;
            return new SubPixel(sp.X, sp.Y, resolution, sp.Dx * fac, sp.Dy * fac);
        }

        public static SubPixel Center(in SubPixel a, in SubPixel b)
        {
            var div = Math.Max(a.Divisions << 1, b.Divisions << 1);
            var utl = ToDivResolution(in a, div);
            var ubr = ToDivResolution(in b, div);
            var xl = utl.X * div + utl.Dx;
            var xr = ubr.X * div + ubr.Dx;
            var yt = utl.Y * div + utl.Dy;
            var yb = ubr.Y * div + ubr.Dy;
            var dx = Math.Min(xl, xr) + Math.Abs(xr - xl) / 2;
            var dy = Math.Min(yt, yb) + Math.Abs(yb - yt) / 2;
            var x = dx / div;
            dx -= x * div;
            var y = dy / div;
            dy -= y * div;
            return Create(x, y, div, dx, dy);
        }

        public static SubPixel ForPixelCenter(int x, int y) => Create(x, y, 2, 1, 1);
    }
}