﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Draw2D.Core;
using Draw2D.Core.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Bounds.Shapes
{
    public abstract class BoxHitTest : HitTestBase
    {
        public override PointShape TryToGetPoint(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            var pointHitTest = hitTest.Registered[typeof(PointShape)];

            if (pointHitTest.TryToGetPoint(box.TopLeft, target, radius, hitTest) != null)
            {
                return box.TopLeft;
            }

            if (box.TryToGetPoint(box.BottomRight, target, radius, hitTest) != null)
            {
                return box.BottomRight;
            }

            foreach (var point in box.Points)
            {
                if (pointHitTest.TryToGetPoint(point, target, radius, hitTest) != null)
                {
                    return point;
                }
            }

            return null;
        }

        public override ShapeObject Contains(ShapeObject shape, Point2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).Contains(target) ? shape : null;
        }

        public override ShapeObject Overlaps(ShapeObject shape, Rect2 target, double radius, IHitTest hitTest)
        {
            var box = shape as BoxShape ?? throw new ArgumentNullException("shape");

            return Rect2.FromPoints(
                box.TopLeft.X,
                box.TopLeft.Y,
                box.BottomRight.X,
                box.BottomRight.Y).IntersectsWith(target) ? shape : null;
        }
    }
}
