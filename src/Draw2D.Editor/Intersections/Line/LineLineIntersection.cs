﻿using System;
using System.Linq;
using Draw2D.Models;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Intersections.Line
{
    public class LineLineIntersection : PointIntersection
    {
        public override void Find(IToolContext context, BaseShape shape)
        {
            var line = shape as LineShape;
            if (line == null)
                throw new ArgumentNullException("shape");

            var lines = context.Container.Shapes.OfType<LineShape>();
            if (lines.Any())
            {
                var a0 = line.Start.ToPoint2();
                var b0 = line.End.ToPoint2();
                foreach (var l in lines)
                {
                    var a1 = l.Start.ToPoint2();
                    var b1 = l.End.ToPoint2();
                    Point2 clip;
                    var intersection = Line2.LineIntersectWithLine(a0, b0, a1, b1, out clip);
                    if (intersection)
                    {
                        var point = new PointShape(clip.X, clip.Y, context.PointShape);
                        Intersections.Add(point);
                        context.WorkingContainer.Shapes.Add(point);
                        context.Renderer.Selected.Add(point);
                    }
                }
            }
        }
    }
}
