﻿using System;
using System.Linq;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class GuideTool : ToolBase
    {
        private enum State { Start, End };
        private State _state = State.Start;
        private LineShape _line = null;

        public override string Name { get { return "Guide"; } }

        public GuideToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y)
        {
            base.LeftDown(context, x, y);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (_state)
            {
                case State.Start:
                    {
                        _line = new LineShape(
                            new PointShape(x, y, null),
                            new PointShape(x, y, null));
                        _line.Style = Settings.GuideStyle;
                        context.WorkingContainer.Shapes.Add(_line);
                        _state = State.End;
                    }
                    break;
                case State.End:
                    {
                        _state = State.Start;
                        _line.End.X = x;
                        _line.End.Y = y;
                        context.WorkingContainer.Shapes.Remove(_line);
                        context.Container.Guides.Add(_line);
                        _line = null;
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
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);
            Filters.ForEach(f => f.Clear(context));
        }
    }
}
