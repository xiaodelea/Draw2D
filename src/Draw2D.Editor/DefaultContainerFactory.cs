﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Draw2D.Input;
using Draw2D.Renderers;
using Draw2D.ViewModels;
using Draw2D.ViewModels.Bounds;
using Draw2D.ViewModels.Containers;
using Draw2D.ViewModels.Filters;
using Draw2D.ViewModels.Intersections;
using Draw2D.ViewModels.Shapes;
using Draw2D.ViewModels.Style;
using Draw2D.ViewModels.Tools;

namespace Draw2D.Editor
{
    public class DefaultContainerFactory : IContainerFactory
    {
        public IStyleLibrary CreateStyleLibrary()
        {
            var styleLibrary = new StyleLibrary()
            {
                Items = new ObservableCollection<ShapeStyle>()
            };

            var fontFamily = "Calibri";

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Default",
                    new ArgbColor(255, 0, 0, 0),
                    new ArgbColor(255, 255, 255, 255),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 0), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Red",
                    new ArgbColor(255, 255, 0, 0),
                    new ArgbColor(255, 255, 0, 0),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 0), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Green",
                    new ArgbColor(255, 0, 255, 0),
                    new ArgbColor(255, 0, 255, 0),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 0), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Blue",
                    new ArgbColor(255, 0, 0, 255),
                    new ArgbColor(255, 0, 0, 255),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Cyan",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Magenta",
                    new ArgbColor(255, 255, 0, 255),
                    new ArgbColor(255, 255, 0, 255),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Yellow",
                    new ArgbColor(255, 255, 255, 0),
                    new ArgbColor(255, 255, 255, 0),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 255, 0), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Black",
                    new ArgbColor(255, 0, 0, 0),
                    new ArgbColor(255, 0, 0, 0),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 0, 0), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Stroke",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    true, false, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Decorator-Fill",
                    new ArgbColor(255, 0, 255, 255),
                    new ArgbColor(255, 0, 255, 255),
                    false, true, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 255, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Guide",
                    new ArgbColor(128, 0, 255, 255),
                    new ArgbColor(128, 0, 255, 255),
                    true, true, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(128, 0, 255, 255), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "Selection",
                    new ArgbColor(255, 0, 120, 215),
                    new ArgbColor(60, 170, 204, 238),
                    true, true, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 0, 120, 215), true)));

            styleLibrary.Items.Add(
                new ShapeStyle(
                    "PointTemplate",
                    new ArgbColor(255, 255, 0, 255),
                    new ArgbColor(255, 255, 0, 255),
                    false, true, true, 2.0,
                    new TextStyle(fontFamily, 12.0, HAlign.Center, VAlign.Center, new ArgbColor(255, 255, 0, 255), true)));

            styleLibrary.CurrentItem = styleLibrary.Items[0];

            return styleLibrary;
        }

        public IGroupLibrary CreateGroupLibrary()
        {
            var groupsLibrary = new GroupLibrary()
            {
                Items = new ObservableCollection<GroupShape>()
            };

            groupsLibrary.CurrentItem = null;

            return groupsLibrary;
        }

        public IToolContext CreateToolContext()
        {
            var editorToolContext = new EditorToolContext(this);

            var hitTest = new HitTest();

            var pathConverter = new SkiaPathConverter();

            var tools = new ObservableCollection<ITool>();

            var noneTool = new NoneTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new SelectionToolSettings()
                {
                    Mode = SelectionMode.Point | SelectionMode.Shape,
                    Targets = SelectionTargets.Shapes,
                    SelectionModifier = Modifier.Control,
                    ConnectionModifier = Modifier.Shift,
                    SelectionStyle = "Selection",
                    ClearSelectionOnClean = false,
                    HitTestRadius = 7.0,
                    ConnectPoints = true,
                    ConnectTestRadius = 10.0,
                    DisconnectPoints = true,
                    DisconnectTestRadius = 10.0
                }
            };

            var pointTool = new PointTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new PointToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var lineTool = new LineTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new LineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    SplitIntersections = false
                }
            };

            var polyLineTool = new PolyLineTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>
                {
                    new LineLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new LineLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new RectangleLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new RectangleLineSettings()
                        {
                            IsEnabled = true
                        }
                    },
                    new EllipseLineIntersection()
                    {
                        Intersections = new ObservableCollection<IPointShape>(),
                        Settings = new EllipseLineSettings()
                        {
                            IsEnabled = true
                        }
                    }
                },
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new PolyLineToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var cubicBezierTool = new CubicBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new CubicBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var quadraticBezierTool = new QuadraticBezierTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new QuadraticBezierToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var conicTool = new ConicTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new ConicToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    Weight = 1.0
                }
            };

            var pathTool = new PathTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new PathToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0,
                    FillType = PathFillType.Winding,
                    IsFilled = true,
                    IsClosed = true
                }
            };

            pathTool.Settings.Tools = new ObservableCollection<ITool>
            {
                new LineTool(),
                new CubicBezierTool(),
                new QuadraticBezierTool(),
                new ConicTool(),
                new MoveTool(pathTool)
            };
            pathTool.Settings.CurrentTool = pathTool.Settings.Tools[0];

            var scribbleTool = new ScribbleTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = false,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new ScribbleToolSettings()
                {
                    Simplify = true,
                    Epsilon = 1.0,
                    FillType = PathFillType.Winding,
                    IsFilled = false,
                    IsClosed = false
                }
            };

            var rectangleTool = new RectangleTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new RectangleToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var ellipseTool = new EllipseTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new EllipseToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            var textTool = new TextTool()
            {
                Intersections = new ObservableCollection<IPointIntersection>(),
                Filters = new ObservableCollection<IPointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new GridSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = "Guide"
                        }
                    },
                    new LineSnapPointFilter()
                    {
                        Guides = new ObservableCollection<IBaseShape>(),
                        Settings = new LineSnapSettings()
                        {
                            IsEnabled = true,
                            EnableGuides = false,
                            Target = LineSnapTarget.Shapes,
                            Mode = LineSnapMode.Point
                            | LineSnapMode.Middle
                            | LineSnapMode.Nearest
                            | LineSnapMode.Intersection
                            | LineSnapMode.Horizontal
                            | LineSnapMode.Vertical,
                            Threshold = 10.0,
                            GuideStyle = "Guide"
                        }
                    }
                },
                Settings = new TextToolSettings()
                {
                    ConnectPoints = true,
                    HitTestRadius = 7.0
                }
            };

            void SetToolDefaults(ITool tool)
            {
                tool.CurrentIntersection = tool.Intersections.Count > 0 ? tool.Intersections[0] : null;
                tool.CurrentFilter = tool.Filters.Count > 0 ? tool.Filters[0] : null;
            }

            SetToolDefaults(noneTool);
            SetToolDefaults(selectionTool);
            SetToolDefaults(pointTool);
            SetToolDefaults(lineTool);
            SetToolDefaults(polyLineTool);
            SetToolDefaults(cubicBezierTool);
            SetToolDefaults(quadraticBezierTool);
            SetToolDefaults(conicTool);
            SetToolDefaults(pathTool);
            SetToolDefaults(scribbleTool);
            SetToolDefaults(rectangleTool);
            SetToolDefaults(ellipseTool);
            SetToolDefaults(textTool);

            tools.Add(noneTool);
            tools.Add(selectionTool);
            tools.Add(pointTool);
            tools.Add(lineTool);
            tools.Add(polyLineTool);
            tools.Add(cubicBezierTool);
            tools.Add(quadraticBezierTool);
            tools.Add(conicTool);
            tools.Add(pathTool);
            tools.Add(scribbleTool);
            tools.Add(rectangleTool);
            tools.Add(ellipseTool);
            tools.Add(textTool);

            editorToolContext.Selection = selectionTool;
            editorToolContext.HitTest = hitTest;
            editorToolContext.PathConverter = pathConverter;
            editorToolContext.CurrentDirectory = null;
            editorToolContext.Files = new ObservableCollection<string>();

            editorToolContext.StyleLibrary = null;
            editorToolContext.GroupLibrary = null;

            var pointTemplate = new RectangleShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
            {
                Points = new ObservableCollection<IPointShape>(),
                Text = new Text(),
                StyleId = "PointTemplate"
            };
            pointTemplate.StartPoint.Owner = pointTemplate;
            pointTemplate.Point.Owner = pointTemplate;

            editorToolContext.PointTemplate = pointTemplate;

            editorToolContext.ContainerViews = new ObservableCollection<IContainerView>();
            editorToolContext.ContainerView = null;
            editorToolContext.Tools = tools;
            editorToolContext.CurrentTool = selectionTool;
            editorToolContext.EditMode = EditMode.Mouse;

            return editorToolContext;
        }

        public IContainerView CreateContainerView(string title)
        {
            var containerView = new ContainerView()
            {
                Title = title,
                Width = 720,
                Height = 630,
                PrintBackground = new ArgbColor(0, 255, 255, 255),
                WorkBackground = new ArgbColor(255, 255, 255, 255),
                InputBackground = new ArgbColor(0, 255, 255, 255),
                CurrentContainer = new CanvasContainer()
                {
                    Points = new ObservableCollection<IPointShape>(),
                    Shapes = new ObservableCollection<IBaseShape>()
                },
                WorkingContainer = null,
                SelectionState = new SelectionState()
                {
                    Hovered = null,
                    Selected = null,
                    Shapes = new HashSet<IBaseShape>()
                },
                ZoomServiceState = new ZoomServiceState()
                {
                    ZoomSpeed = 1.2,
                    ZoomX = double.NaN,
                    ZoomY = double.NaN,
                    OffsetX = double.NaN,
                    OffsetY = double.NaN,
                    IsPanning = false,
                    IsZooming = false,
                    InitFitMode = FitMode.Center,
                    AutoFitMode = FitMode.None
                },
                ContainerPresenter = null,
                InputService = null,
                ZoomService = null
            };

            return containerView;
        }
    }
}