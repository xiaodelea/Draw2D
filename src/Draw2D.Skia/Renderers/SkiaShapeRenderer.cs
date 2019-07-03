﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;
using SkiaSharp;

namespace Draw2D.Renderers
{
    public class SkiaShapeRenderer : IShapeRenderer
    {
        private IToolContext _context;
        private ISelectionState _selectionState;
        private Dictionary<Typeface, SKTypeface> _typefaceCache;
        private Dictionary<TextStyle, (SKPaint paint, SKFontMetrics metrics)> _textPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _fillPaintCache;
        private Dictionary<ShapeStyle, SKPaint> _strokePaintCache;

        public SkiaShapeRenderer(IToolContext context, ISelectionState selectionState)
        {
            _context = context;
            _selectionState = selectionState;
            _typefaceCache = new Dictionary<Typeface, SKTypeface>();
            _textPaintCache = new Dictionary<TextStyle, (SKPaint paint, SKFontMetrics metrics)>();
            _fillPaintCache = new Dictionary<ShapeStyle, SKPaint>();
            _strokePaintCache = new Dictionary<ShapeStyle, SKPaint>();
        }

        public void Dispose()
        {
            _context = null;
            _selectionState = null;

            if (_typefaceCache != null)
            {
                foreach (var cache in _typefaceCache)
                {
                    cache.Value.Dispose();
                }
                _typefaceCache = null;
            }

            if (_textPaintCache != null)
            {
                foreach (var cache in _textPaintCache)
                {
                    cache.Value.paint.Dispose();
                }
                _textPaintCache = null;
            }

            if (_fillPaintCache != null)
            {
                foreach (var cache in _fillPaintCache)
                {
                    cache.Value.Dispose();
                }
                _fillPaintCache = null;
            }

            if (_strokePaintCache != null)
            {
                foreach (var cache in _strokePaintCache)
                {
                    cache.Value.Dispose();
                }
                _strokePaintCache = null;
            }
        }

        private void GetSKTypeface(Typeface style, out SKTypeface typeface)
        {
            if (style.IsDirty == true || !_typefaceCache.TryGetValue(style, out typeface))
            {
                style.Invalidate();
                typeface = SkiaHelper.ToSKTypeface(style);
                _typefaceCache[style] = typeface;
#if DEBUG_DICT_CACHE
                Log.WriteLine($"ToSKTypeface: ctor()");
#endif
            }
        }

        private void GetSKPaintFill(ShapeStyle style, out SKPaint brush)
        {
            if (style.Fill.IsDirty == true || !_fillPaintCache.TryGetValue(style, out var brushCached))
            {
                style.Fill.Invalidate();
                brushCached = SkiaHelper.ToSKPaintBrush(style.Fill, style.IsAntialias);
                _fillPaintCache[style] = brushCached;
#if DEBUG_DICT_CACHE
                Log.WriteLine($"ToSKPaintBrush: ctor()");
#endif
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(brushCached, style.Fill);
            }

            brush = brushCached;
        }

        private void GetSKPaintStroke(ShapeStyle style, out SKPaint pen, double scale)
        {
            if (style.IsDirty == true || style.Stroke.IsDirty == true || !_strokePaintCache.TryGetValue(style, out var penCached))
            {
                style.Invalidate();
                style.Stroke.Invalidate();
                penCached = SkiaHelper.ToSKPaintPen(style, scale);
                _strokePaintCache[style] = penCached;
#if DEBUG_DICT_CACHE
                Log.WriteLine($"ToSKPaintPen: ctor()");
#endif
            }
            else
            {
                SkiaHelper.ToSKPaintPenUpdate(penCached, style, scale);
            }

            pen = penCached;
        }

        private void GetSKPaintStrokeText(TextStyle style, out SKPaint paint, out SKFontMetrics metrics)
        {
            (SKPaint paint, SKFontMetrics metrics) cached;
            cached.paint = null;
            cached.metrics = default;

            if (style.IsDirty == true || style.Stroke.IsDirty == true || style.Typeface.IsDirty == true || !_textPaintCache.TryGetValue(style, out cached))
            {
                style.Invalidate();
                style.Stroke.Invalidate();
                GetSKTypeface(style.Typeface, out var typeface);

                cached.paint = SkiaHelper.ToSKPaintBrush(style);
                cached.paint.Typeface = typeface;
                cached.paint.TextEncoding = SKTextEncoding.Utf16;
                cached.paint.TextSize = (float)style.FontSize;
#if DEBUG_DICT_CACHE
                Log.WriteLine($"ToSKPaintBrush: ctor()");
#endif
                switch (style.HAlign)
                {
                    default:
                    case HAlign.Left:
                        cached.paint.TextAlign = SKTextAlign.Left;
                        break;
                    case HAlign.Center:
                        cached.paint.TextAlign = SKTextAlign.Center;
                        break;
                    case HAlign.Right:
                        cached.paint.TextAlign = SKTextAlign.Right;
                        break;
                }

                cached.metrics = cached.paint.FontMetrics;
                _textPaintCache[style] = cached;
            }
            else
            {
                SkiaHelper.ToSKPaintBrushUpdate(cached.paint, style.Stroke);
            }

            paint = cached.paint;
            metrics = cached.metrics;
        }

        private void DrawText(SKCanvas canvas, Text text, IPointShape topLeft, IPointShape bottomRight, ShapeStyle style, double dx, double dy, double scale)
        {
            var rect = SkiaHelper.ToSKRect(topLeft, bottomRight, dx, dy);
            GetSKPaintStrokeText(style.TextStyle, out var paint, out var metrics);
#if DEBUG_DRAW_TEXT
            var mBaseline = 0.0f;
#endif
            var mTop = metrics.Top;
            var mAscent = metrics.Ascent;
            var mDescent = metrics.Descent;
            var mBottom = metrics.Bottom;
            var mLeading = metrics.Leading;
            var mCapHeight = metrics.CapHeight;
            var mLineHeight = metrics.Bottom - metrics.Top;
            var mXMax = metrics.XMax;
            var mXMin = metrics.XMin;

            float x = rect.Left;
            float y = rect.Top;
            float width = rect.Width;
            float height = rect.Height;

            switch (style.TextStyle.VAlign)
            {
                default:
                case VAlign.Top:
                    y -= mAscent;
                    break;
                case VAlign.Center:
                    y += (height / 2.0f) - (mAscent / 2.0f) - mDescent / 2.0f;
                    break;
                case VAlign.Bottom:
                    y += height - mDescent;
                    break;
            }
#if DEBUG_DRAW_TEXT
            using (var boundsPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(255, 255, 255, 255) })
            using (var mTopPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(128, 0, 128, 255) })
            using (var mAscentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(0, 255, 0, 255) })
            using (var mBaselinePen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(255, 0, 0, 255) })
            using (var mDescentPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(0, 0, 255, 255) })
            using (var mBottomPen = new SKPaint() { IsAntialias = true, IsStroke = true, StrokeWidth = (float)(style.StrokeWidth / scale), Color = new SKColor(255, 127, 0, 255) })
            {
                var bounds = new SKRect();
                paint.MeasureText(text.Value, ref bounds);
                var boundsAdjusted = new SKRect(x + bounds.Left, y + bounds.Top, x + bounds.Right, y + bounds.Bottom);
                canvas.DrawRect(boundsAdjusted, boundsPen);
                canvas.DrawLine(new SKPoint(x, y + mTop), new SKPoint(x + width, y + mTop), mTopPen);
                canvas.DrawLine(new SKPoint(x, y + mAscent), new SKPoint(x + width, y + mAscent), mAscentPen);
                canvas.DrawLine(new SKPoint(x, y + mBaseline), new SKPoint(x + width, y + mBaseline), mBaselinePen);
                canvas.DrawLine(new SKPoint(x, y + mDescent), new SKPoint(x + width, y + mDescent), mDescentPen);
                canvas.DrawLine(new SKPoint(x, y + mBottom), new SKPoint(x + width, y + mBottom), mBottomPen);
            }
#endif
            switch (style.TextStyle.HAlign)
            {
                default:
                case HAlign.Left:
                    // x = x;
                    break;
                case HAlign.Center:
                    x += width / 2.0f;
                    break;
                case HAlign.Right:
                    x += width;
                    break;
            }
#if DEBUG_DRAW_TEXT
            int line = 2;
            canvas.DrawText($"Top: {mTop}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Ascent: {mAscent}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Baseline: {mBaseline}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Descent: {mDescent}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Bottom: {mBottom}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"Leading: {mLeading}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"CapHeight: {mCapHeight}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"LineHeight: {mLineHeight}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"XMax: {mXMax}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"XMin: {mXMin}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"x: {x}", x, y + mLineHeight * line++, paint);
            canvas.DrawText($"y: {y}", x, y + mLineHeight * line++, paint);
#endif
            canvas.DrawText(text.Value, x, y, paint);
        }

        private void DrawTextOnPath(SKCanvas canvas, SKPath path, Text text, TextStyle style)
        {
            GetSKPaintStrokeText(style, out var paint, out var metrics);
            var bounds = new SKRect();
            float baseTextWidth = paint.MeasureText(text.Value, ref bounds);
            SKPathMeasure pathMeasure = new SKPathMeasure(path, false, 1);
            float hOffset = (pathMeasure.Length / 2f) - (baseTextWidth / 2f);
            canvas.DrawTextOnPath(text.Value, path, hOffset, metrics.Bottom - metrics.Top, paint);
        }

        public void DrawLine(object dc, LineShape line, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_LINE
            if (style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, scale);
                    canvas.DrawLine(SkiaHelper.ToPoint(line.StartPoint, dx, dy), SkiaHelper.ToPoint(line.Point, dx, dy), pen);
                }
            }
#else
            if (style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddLine(null, line, dx, dy, geometry);
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(line.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, line.Text, style.TextStyle);
                    }
                }
            }
#endif
        }

        public void DrawCubicBezier(object dc, CubicBezierShape cubicBezier, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddCubic(null, cubicBezier, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(cubicBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, cubicBezier.Text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawQuadraticBezier(object dc, QuadraticBezierShape quadraticBezier, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddQuad(null, quadraticBezier, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(quadraticBezier.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, quadraticBezier.Text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawConic(object dc, ConicShape conic, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddConic(null, conic, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(conic.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, conic.Text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawPath(object dc, PathShape path, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SkiaHelper.ToSKPathFillType(path.FillType) })
                {
                    SkiaHelper.AddPath(null, path, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(path.Text?.Value))
                    {
                        DrawTextOnPath(canvas, geometry, path.Text, style.TextStyle);
                    }
                }
            }
        }

        public void DrawRectangle(object dc, RectangleShape rectangle, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_RECT
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToRect(rectangle.StartPoint, rectangle.Point, dx, dy);
                if (style.IsFilled)
                {
                    GetSKPaintFill(style, out var brush);
                    canvas.DrawRect(rect, brush);
                }
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, scale);
                    canvas.DrawRect(rect, pen);
                }
                if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(rectangle.Text?.Value))
                {
                    DrawText(canvas, rectangle.Text, rectangle.StartPoint, rectangle.Point, style, dx, dy, scale);
                }
            }
#else
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddRect(null, rectangle, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(rectangle.Text?.Value))
                    {
                        DrawText(canvas, rectangle.Text, rectangle.StartPoint, rectangle.Point, style, dx, dy, scale);
                    }
                }
            }
#endif
        }

        public void DrawEllipse(object dc, EllipseShape ellipse, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
#if USE_DRAW_OVAL
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                var rect = SkiaHelper.ToRect(ellipse.StartPoint, ellipse.Point, dx, dy);
                if (style.IsFilled)
                {
                    GetSKPaintFill(style, out var brush);
                    canvas.DrawOval(rect, brush);
                }
                if (style.IsStroked)
                {
                    GetSKPaintStroke(style, out var pen, scale);
                    canvas.DrawOval(rect, pen);
                }
                if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(ellipse.Text?.Value))
                {
                    DrawText(canvas, ellipse.Text, ellipse.StartPoint, ellipse.Point, style, dx, dy, scale);
                }
            }
#else
            if (style.IsFilled || style.IsStroked || style.TextStyle.IsStroked)
            {
                var canvas = dc as SKCanvas;
                using (var geometry = new SKPath() { FillType = SKPathFillType.Winding })
                {
                    SkiaHelper.AddOval(null, ellipse, dx, dy, geometry);
                    if (style.IsFilled)
                    {
                        GetSKPaintFill(style, out var brush);
                        canvas.DrawPath(geometry, brush);
                    }
                    if (style.IsStroked)
                    {
                        GetSKPaintStroke(style, out var pen, scale);
                        canvas.DrawPath(geometry, pen);
                    }
                    if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(ellipse.Text?.Value))
                    {
                        DrawText(canvas, ellipse.Text, ellipse.StartPoint, ellipse.Point, style, dx, dy, scale);
                    }
                }
            }
#endif
        }

        public void DrawText(object dc, TextShape text, string styleId, double dx, double dy, double scale)
        {
            var style = _context?.StyleLibrary?.Get(styleId);
            if (style == null)
            {
                return;
            }
            if (style.TextStyle.IsStroked && !string.IsNullOrEmpty(text.Text?.Value))
            {
                var canvas = dc as SKCanvas;
                DrawText(canvas, text.Text, text.StartPoint, text.Point, style, dx, dy, scale);
            }
        }
    }
}