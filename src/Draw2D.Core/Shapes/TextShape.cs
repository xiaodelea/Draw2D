﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Diagnostics;
using Draw2D.Core.Renderers;

namespace Draw2D.Core.Shapes
{
    public class TextShape : ConnectableShape, ICopyable
    {
        private PointShape _topLeft;
        private PointShape _bottomRight;
        private TextObject _text;

        public PointShape TopLeft
        {
            get => _topLeft;
            set => Update(ref _topLeft, value);
        }

        public PointShape BottomRight
        {
            get => _bottomRight;
            set => Update(ref _bottomRight, value);
        }

        public TextObject Text
        {
            get => _text;
            set => Update(ref _text, value);
        }

        public TextShape()
            : base()
        {
        }

        public TextShape(PointShape topLeft, PointShape bottomRight)
            : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
        }

        public TextShape(TextObject text, PointShape topLeft, PointShape bottomRight) : base()
        {
            this.TopLeft = topLeft;
            this.BottomRight = bottomRight;
            this.Text = text;
        }

        public override IEnumerable<PointShape> GetPoints()
        {
            yield return TopLeft;
            yield return BottomRight;
            foreach (var point in Points)
            {
                yield return point;
            }
        }

        public override bool Invalidate(ShapeRenderer r, double dx, double dy)
        {
            bool result = base.Invalidate(r, dx, dy);

            if (_text?.IsDirty ?? false)
            {
                _text.IsDirty = false;
                result |= true;
            }

            result |= _topLeft?.Invalidate(r, dx, dy) ?? false;
            result |= _bottomRight?.Invalidate(r, dx, dy) ?? false;

            if (this.IsDirty || result == true)
            {
                r.InvalidateCache(this, Style, dx, dy);
                this.IsDirty = false;
                result |= true;
            }

            return result;
        }

        public override void Draw(object dc, ShapeRenderer r, double dx, double dy)
        {
            base.BeginTransform(dc, r);

            if (Style != null)
            {
                r.DrawText(dc, this, Style, dx, dy);
            }

            if (r.Selected.Contains(_topLeft))
            {
                _topLeft.Draw(dc, r, dx, dy);
            }

            if (r.Selected.Contains(_bottomRight))
            {
                _bottomRight.Draw(dc, r, dx, dy);
            }

            base.Draw(dc, r, dx, dy);
            base.EndTransform(dc, r);
        }

        public override void Move(ISet<ShapeObject> selected, double dx, double dy)
        {
            if (!selected.Contains(_topLeft))
            {
                _topLeft.Move(selected, dx, dy);
            }

            if (!selected.Contains(_bottomRight))
            {
                _bottomRight.Move(selected, dx, dy);
            }

            base.Move(selected, dx, dy);
        }

        public override void Select(ISet<ShapeObject> selected)
        {
            base.Select(selected);
            TopLeft.Select(selected);
            BottomRight.Select(selected);
        }

        public override void Deselect(ISet<ShapeObject> selected)
        {
            base.Deselect(selected);
            TopLeft.Deselect(selected);
            BottomRight.Deselect(selected);
        }

        private bool CanConnect(PointShape point)
        {
            return TopLeft != point
                && BottomRight != point;
        }

        public override bool Connect(PointShape point, PointShape target)
        {
            if (base.Connect(point, target))
            {
                return true;
            }
            else if (CanConnect(point))
            {
                if (TopLeft == target)
                {
                    Debug.WriteLine($"{nameof(TextShape)}: Connected to {nameof(TopLeft)}");
                    this.TopLeft = point;
                    return true;
                }
                else if (BottomRight == target)
                {
                    Debug.WriteLine($"{nameof(TextShape)}: Connected to {nameof(BottomRight)}");
                    this.BottomRight = point;
                    return true;
                }
            }
            return false;
        }

        public override bool Disconnect(PointShape point, out PointShape result)
        {
            if (base.Disconnect(point, out result))
            {
                return true;
            }
            else if (TopLeft == point)
            {
                Debug.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(TopLeft)}");
                result = (PointShape)point.Copy(null);
                this.TopLeft = result;
                return true;
            }
            else if (BottomRight == point)
            {
                Debug.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(BottomRight)}");
                result = (PointShape)point.Copy(null);
                this.BottomRight = result;
                return true;
            }
            result = null;
            return false;
        }

        public override bool Disconnect()
        {
            bool result = base.Disconnect();

            if (this.TopLeft != null)
            {
                Debug.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(TopLeft)}");
                this.TopLeft = (PointShape)this.TopLeft.Copy(null);
                result = true;
            }

            if (this.BottomRight != null)
            {
                Debug.WriteLine($"{nameof(TextShape)}: Disconnected from {nameof(BottomRight)}");
                this.BottomRight = (PointShape)this.BottomRight.Copy(null);
                result = true;
            }

            return result;
        }

        public object Copy(IDictionary<object, object> shared)
        {
            var copy = new TextShape()
            {
                Style = this.Style,
                Transform = (MatrixObject)this.Transform?.Copy(shared),
                Text = (TextObject)this.Text?.Copy(shared)
            };

            if (shared != null)
            {
                copy.TopLeft = (PointShape)shared[this.TopLeft];
                copy.BottomRight = (PointShape)shared[this.BottomRight];

                foreach (var point in this.Points)
                {
                    copy.Points.Add((PointShape)shared[point]);
                }
            }

            return copy;
        }
    }
}
