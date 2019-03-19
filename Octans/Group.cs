﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Octans
{
    public class Group : ShapeBase
    {
        private readonly List<IShape> _children;
        private readonly Lazy<Bounds> _bounds;

        public Group()
        {
            _children = new List<IShape>();
            _bounds = new Lazy<Bounds>(BoundsFactory);
        }

        public Group(params IShape[] shapes)
        {
            _children = new List<IShape>(shapes);
            _bounds = new Lazy<Bounds>(BoundsFactory);
        }

        public IReadOnlyList<IShape> Children => _children;

        public override IIntersections LocalIntersects(in Ray localRay)
        {
            var bounds = LocalBounds();
            if (!bounds.DoesIntersect(localRay))
            {
                return Intersections.Empty();
            }

            var intersections = Intersections.Builder();
            foreach (var child in _children)
            {
                intersections.AddRange(child.Intersects(in localRay));
            }

            return intersections.ToIntersections();
        }

        public override Vector LocalNormalAt(in Point localPoint, in Intersection intersection) => throw new NotImplementedException();

        public override Bounds LocalBounds()
        {
            return _bounds.Value;
        }

        private Bounds BoundsFactory()
        {
            return Children.Aggregate(Bounds.Empty, (current, child) => current + child.ParentSpaceBounds());
        }

        public void AddChild(IShape shape)
        {
            _children.Add(shape);
            shape.Parent = this;
        }
    }
}