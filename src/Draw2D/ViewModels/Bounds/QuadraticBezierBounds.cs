﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.ViewModels.Shapes;
using Spatial;

namespace Draw2D.ViewModels.Bounds
{
    public class QuadraticBezierBounds : IBounds
    {
        public Type TargetType => typeof(QuadraticBezierShape);

        public PointShape TryToGetPoint(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var quadraticBezier = shape as QuadraticBezierShape;
            if (quadraticBezier == null)
                throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(quadraticBezier.StartPoint, target, radius, hitTest) != null)
            {
                return quadraticBezier.StartPoint;
            }

            if (pointHitTest.TryToGetPoint(quadraticBezier.Point1, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point1;
            }

            if (pointHitTest.TryToGetPoint(quadraticBezier.Point2, target, radius, hitTest) != null)
            {
                return quadraticBezier.Point2;
            }

            foreach (var point in quadraticBezier.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public BaseShape Contains(BaseShape shape, Point2 target, double radius, IHitTest hitTest)
        {
            var quadraticBezier = shape as QuadraticBezierShape;
            if (quadraticBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Contains(quadraticBezier.GetPoints(), target) ? shape : null;
        }

        public BaseShape Overlaps(BaseShape shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var quadraticBezier = shape as QuadraticBezierShape;
            if (quadraticBezier == null)
                throw new ArgumentNullException("shape");

            return HitTestHelper.Overlap(quadraticBezier.GetPoints(), target) ? shape : null;
        }
    }
}