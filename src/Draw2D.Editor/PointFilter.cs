﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using Draw2D.Core;

namespace Draw2D.Editor
{
    public abstract class PointFilter
    {
        public abstract string Name { get; }
        public List<ShapeObject> Guides { get; set; }

        protected PointFilter()
        {
            Guides = new List<ShapeObject>();
        }

        public abstract bool Process(IToolContext context, ref double x, ref double y);

        public virtual void Clear(IToolContext context)
        {
            foreach (var guide in Guides)
            {
                context.WorkingContainer.Shapes.Remove(guide);
                context.Selected.Remove(guide);
            }
            Guides.Clear();
        }
    }
}