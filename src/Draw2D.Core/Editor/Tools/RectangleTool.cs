﻿using System.Linq;
using Draw2D.Models.Shapes;

namespace Draw2D.Editor.Tools
{
    public class RectangleTool : ToolBase
    {
        private RectangleShape _rectangle = null;

        public enum State { TopLeft, BottomRight };
        public State CurrentState = State.TopLeft;

        public override string Name { get { return "Rectangle"; } }

        public RectangleToolSettings Settings { get; set; }

        public override void LeftDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.LeftDown(context, x, y, modifier);

            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.TopLeft:
                    {
                        _rectangle = new RectangleShape(
                            context.GetNextPoint(x, y, Settings.ConnectPoints, Settings.HitTestRadius),
                            context.GetNextPoint(x, y, false, 0.0));
                        _rectangle.Style = context.CurrentStyle;
                        context.WorkingContainer.Shapes.Add(_rectangle);
                        context.Selected.Add(_rectangle.TopLeft);
                        context.Selected.Add(_rectangle.BottomRight);
                        CurrentState = State.BottomRight;
                    }
                    break;
                case State.BottomRight:
                    {
                        CurrentState = State.TopLeft;
                        context.Selected.Remove(_rectangle.BottomRight);
                        _rectangle.BottomRight = context.GetNextPoint(x, y, Settings.ConnectPoints, Settings.HitTestRadius);
                        _rectangle.BottomRight.Y = y;
                        context.WorkingContainer.Shapes.Remove(_rectangle);
                        context.Selected.Remove(_rectangle.TopLeft);
                        context.CurrentContainer.Shapes.Add(_rectangle);
                        _rectangle = null;
                    }
                    break;
            }
        }

        public override void RightDown(IToolContext context, double x, double y, Modifier modifier)
        {
            base.RightDown(context, x, y, modifier);

            switch (CurrentState)
            {
                case State.BottomRight:
                    {
                        this.Clean(context);
                    }
                    break;
            }
        }

        public override void Move(IToolContext context, double x, double y, Modifier modifier)
        {
            base.Move(context, x, y, modifier);

            Filters.ForEach(f => f.Clear(context));
            Filters.Any(f => f.Process(context, ref x, ref y));

            switch (CurrentState)
            {
                case State.BottomRight:
                    {
                        _rectangle.BottomRight.X = x;
                        _rectangle.BottomRight.Y = y;
                    }
                    break;
            }
        }

        public override void Clean(IToolContext context)
        {
            base.Clean(context);

            CurrentState = State.TopLeft;

            Filters.ForEach(f => f.Clear(context));

            if (_rectangle != null)
            {
                context.WorkingContainer.Shapes.Remove(_rectangle);
                context.Selected.Remove(_rectangle.TopLeft);
                context.Selected.Remove(_rectangle.BottomRight);
                _rectangle = null;
            }
        }
    }
}