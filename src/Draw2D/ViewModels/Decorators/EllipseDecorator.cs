﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Draw2D.ViewModels.Renderer;
using Draw2D.ViewModels.Shapes;

namespace Draw2D.ViewModels.Decorators
{
    public class EllipseDecorator : CommonDecorator
    {
        public override void Draw(object dc, IShapeRenderer renderer, BaseShape shape, ISelection selection, double dx, double dy)
        {
        }
    }
}