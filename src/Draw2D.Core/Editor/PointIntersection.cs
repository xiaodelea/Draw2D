﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core.Shapes;

namespace Draw2D.Core.Editor
{
    public abstract class PointIntersection
    {
        public abstract string Name { get; }
        public List<PointShape> Intersections { get; set; }

        protected PointIntersection()
        {
            Intersections = new List<PointShape>();
        }

        public abstract void Find(IToolContext context, ShapeObject shape);

        public virtual void Clear(IToolContext context)
        {
            foreach (var point in Intersections)
            {
                context.WorkingContainer.Shapes.Remove(point);
                context.Selected.Remove(point);
            }
            Intersections.Clear();
        }
    }
}