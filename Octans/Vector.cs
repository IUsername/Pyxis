﻿using System;

namespace Octans
{
    public readonly struct Vector : IEquatable<Vector>
    {
        private const float Epsilon = 0.00001f;

        public readonly float X;
        public readonly float Y;
        public readonly float Z;
        public readonly float W;

        private Vector(float x, float y, float z, float w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        public Vector(float x, float y, float z) : this(x, y, z, 0.0f)
        {
        }

        public Vector ZeroW()
        {
            return new Vector(X, Y, Z);
        }

        public Vector Add(Vector t) => new Vector(X + t.X, Y + t.Y, Z + t.Z, W + t.W);

        public Vector Subtract(Vector t) => new Vector(X - t.X, Y - t.Y, Z - t.Z, W - t.W);

        public Vector Negate() => new Vector(-X, -Y, -Z, -W);

        public Vector Scale(float scalar) => new Vector(X * scalar, Y * scalar, Z * scalar, W * scalar);

        public Vector Divide(float scalar) => new Vector(X / scalar, Y / scalar, Z / scalar, W / scalar);

        public float Magnitude() => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);

        public Vector Normalize()
        {
            var m = Magnitude();
            return new Vector(X / m, Y / m, Z / m, W / m);
        }

        public Vector Reflect(in Vector normal) => Reflect(this, normal);

        public bool Equals(Vector other) =>
            Check.Within(X, other.X, Epsilon)
            && Check.Within(Y, other.Y, Epsilon)
            && Check.Within(Z, other.Z, Epsilon)
            && Check.Within(W, other.W, Epsilon);

        public override bool Equals(object obj) => !(obj is null) && obj is Vector other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        public static Vector operator +(Vector left, Vector right) => left.Add(right);

        public static Vector operator -(Vector left, Vector right) => left.Subtract(right);

        public static Vector operator *(Vector t, float scalar) => t.Scale(scalar);

        public static Vector operator *(float scalar, Vector t) => t.Scale(scalar);

        public static Vector operator /(Vector t, float scalar) => t.Divide(scalar);

        public static Vector operator -(Vector t) => t.Negate();

        public static bool operator ==(Vector left, Vector right) => left.Equals(right);

        public static bool operator !=(Vector left, Vector right) => !left.Equals(right);

        public static float operator %(Vector left, Vector right) => Dot(left, right);

        public static float Dot(in Vector a, in Vector b) => a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;

        public static Vector Cross(in Vector a, in Vector b) =>
            new Vector(a.Y * b.Z - a.Z * b.Y,
                       a.Z * b.X - a.X * b.Z,
                       a.X * b.Y - a.Y * b.X);

        public static Vector Reflect(in Vector @in, in Vector normal) => @in - normal * 2f * Dot(@in, normal);
    }
}