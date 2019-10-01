﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//
// Parts of this source file are adapted from the https://github.com/vvvv/SVG
using SkiaSharp;
using Svg;

namespace Svg.Skia
{
    internal struct Text
    {
        public SKMatrix matrix;

        public Text(SvgText svgText)
        {
            matrix = SvgHelper.GetSKMatrix(svgText.Transforms);
        }
    }
}