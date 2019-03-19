﻿using System;

namespace Octans
{
    public class Sphere : ShapeBase
    {
        public override IIntersections LocalIntersects(in Ray localRay)
        {
            var sphereToRay = localRay.Origin - Point.Zero;
            var a = localRay.Direction % localRay.Direction;
            var b = 2f * localRay.Direction % sphereToRay;
            var c = sphereToRay % sphereToRay - 1f;
            var discriminant = b * b - 4 * a * c;
            if (discriminant < 0f)
            {
                return Intersections.Empty();
            }

            var t1 = (-b - MathF.Sqrt(discriminant)) / (2f * a);
            var t2 = (-b + MathF.Sqrt(discriminant)) / (2f * a);
            return Intersections.Create(
                new Intersection(t1, this),
                new Intersection(t2, this));
        }

        public override Vector LocalNormalAt(in Point localPoint, in Intersection intersection) =>
            localPoint - Point.Zero;

        public override Bounds LocalBounds() => Bounds.Unit;
    }
}