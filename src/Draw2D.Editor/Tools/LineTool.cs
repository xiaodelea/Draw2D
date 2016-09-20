﻿using System;
using System.Collections.Generic;
using System.Linq;
using Draw2D.Models.Shapes;
using Draw2D.Spatial;

namespace Draw2D.Editor.Tools
{
    public class LineTool : ToolBase
    {
        private enum State { Start, End };
        private State _state = State.Start;
        private LineShape _line = null;

        public override string Name { get { return "Line"; } }

        public LineToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y)
        {
            base.LeftDown(context, x, y);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.Start:
                    {
                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.Container.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        _line = new LineShape(
                            point ?? new PointShape(x, y, context.PointShape),
                            new PointShape(x, y, context.PointShape));
                        _line.Style = context.Style;
                        context.WorkingContainer.Shapes.Add(_line);
                        context.Renderer.Selected.Add(_line.Start);
                        context.Renderer.Selected.Add(_line.End);
                        _state = State.End;
                    }
                    break;
                case State.End:
                    {
                        _state = State.Start;

                        PointShape point = null;
                        if (Settings.ConnectPoints)
                        {
                            point = context.HitTest.TryToGetPoint(context.Container.Shapes, new Point2(x, y), Settings.HitTestRadius);
                        }

                        if (point != null)
                        {
                            _line.End = point;
                        }
                        else
                        {
                            _line.End.X = x;
                            _line.End.Y = y;
                        }

                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Renderer.Selected.Remove(_line.Start);
                        context.Renderer.Selected.Remove(_line.End);

                        Intersections.ForEach(i => i.Clear(context));
                        Intersections.ForEach(i => i.Find(context, _line));

                        if (Settings.SplitIntersections && Intersections.Any(i => i.Intersections.Count > 0))
                        {
                            LineTool.SplitByIntersections(context, Intersections, _line);
                        }
                        else
                        {
                            context.Container.Shapes.Add(_line);
                        }

                        _line = null;
                        Intersections.ForEach(i => i.Clear(context));
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y)
        {
            base.RightDown(context, x, y);

            switch (_state)
            {
                case State.End:
                    {
                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Renderer.Selected.Remove(_line.Start);
                        context.Renderer.Selected.Remove(_line.End);
                        _line = null;
                        _state = State.Start;
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y)
        {
            base.Move(context, x, y);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.End:
                    {
                        _line.End.X = x;
                        _line.End.Y = y;
                        Intersections.ForEach(i => i.Clear(context));
                        Intersections.ForEach(i => i.Find(context, _line));
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);
            Intersections.ForEach(i => i.Clear(context));
            Filters.ForEach(f => f.Clear(context));
        }

        public static void SplitByIntersections(IToolContext context, IEnumerable<PointIntersection> intersections, LineShape target)
        {
            var points = intersections.SelectMany(i => i.Intersections).ToList();
            points.Insert(0, target.Start);
            points.Insert(points.Count, target.End);

            // Order points from Start to End and take only unique positions.
            var unique = points
                .Select(p => new Point2(p.X, p.Y)).Distinct().OrderBy(p => p)
                .Select(p => new PointShape(p.X, p.Y, context.PointShape)).ToList();

            for (int i = 0; i < unique.Count - 1; i++)
            {
                var line = new LineShape(unique[i], unique[i + 1]);
                line.Style = context.Style;
                context.Container.Shapes.Add(line);
            }
        }
    }
}
