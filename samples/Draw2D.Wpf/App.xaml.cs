﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using Draw2D.Editor;
using Draw2D.Editor.Bounds;
using Draw2D.Editor.Bounds.Shapes;
using Draw2D.Editor.Filters;
using Draw2D.Editor.Intersections.Line;
using Draw2D.Editor.Selection;
using Draw2D.Editor.Tools;
using Draw2D.Models.Containers;
using Draw2D.Models.Shapes;
using Draw2D.Models.Style;
using Draw2D.ViewModels.Containers;
using Draw2D.Wpf.Renderers;
using Draw2D.Wpf.Views;

namespace Draw2D.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // HITTEST

            var hitTest = new HitTest();
            
            hitTest.Register(new PointHitTest());
            hitTest.Register(new LineHitTest());
            hitTest.Register(new RectangleHitTest());
            hitTest.Register(new EllipseHitTest());
            hitTest.Register(new GroupHitTest());

            // FILTERS

            var gridSnapPointFilter = new GridSnapPointFilter()
            {
                Settings = new GridSnapSettings()
                {
                    EnableGuides = true,
                    Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                    GridSizeX = 15.0,
                    GridSizeY = 15.0,
                    GuideStyle = new DrawStyle(new DrawColor(128, 0, 255, 255), new DrawColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            var lineSnapPointFilter = new LineSnapPointFilter()
            {
                Settings = new LineSnapSettings()
                {
                    EnableGuides = true,
                    Target = LineSnapTarget.Guides | LineSnapTarget.Shapes,
                    Mode = LineSnapMode.Point
                    | LineSnapMode.Middle
                    | LineSnapMode.Nearest
                    | LineSnapMode.Intersection
                    | LineSnapMode.Horizontal
                    | LineSnapMode.Vertical,
                    Treshold = 10.0,
                    GuideStyle = new DrawStyle(new DrawColor(128, 0, 255, 255), new DrawColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            // TOOLS

            var tools = new ObservableCollection<ToolBase>();

            var noneTool = new NoneTool()
            {
                Settings = new NoneToolSettings()
            };

            var selectionTool = new SelectionTool()
            {
                Filters = new List<PointFilter>
                {
                    new GridSnapPointFilter()
                    {
                        Settings = new GridSnapSettings()
                        {
                            EnableGuides = false,
                            Mode = GridSnapMode.Horizontal | GridSnapMode.Vertical,
                            GridSizeX = 15.0,
                            GridSizeY = 15.0,
                            GuideStyle = null
                        }
                    }
                },
                Settings = new SelectionToolSettings()
                {
                    Mode = SelectionMode.Point | SelectionMode.Shape,
                    Targets = SelectionTargets.Shapes | SelectionTargets.Guides,
                    SelectionStyle = new DrawStyle(new DrawColor(128, 0, 0, 255), new DrawColor(80, 0, 0, 255), 2.0, true, true),
                    HitTestRadius = 7.0
                }
            };

            var guideTool = new GuideTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new GuideToolSettings()
                {
                    GuideStyle = new DrawStyle(new DrawColor(128, 0, 255, 255), new DrawColor(128, 0, 255, 255), 2.0, true, true)
                }
            };

            var pointTool = new PointTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new PointToolSettings()
            };

            var lineTool = new LineTool()
            {
                Intersections = new List<PointIntersection>
                {
                    new LineLineIntersection(),
                    new RectangleLineIntersection(),
                    new EllipseLineIntersection()
                },
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
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
                Intersections = new List<PointIntersection>
                {
                    new LineLineIntersection(),
                    new RectangleLineIntersection(),
                    new EllipseLineIntersection()
                },
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new PolyLineToolSettings()
            };

            var rectangleTool = new RectangleTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new RectangleToolSettings()
            };

            var ellipseTool = new EllipseTool()
            {
                Filters = new List<PointFilter>
                {
                    lineSnapPointFilter,
                    gridSnapPointFilter
                },
                Settings = new EllipseToolSettings()
            };

            tools.Add(noneTool);
            tools.Add(selectionTool);
            tools.Add(guideTool);
            tools.Add(pointTool);
            tools.Add(lineTool);
            tools.Add(polyLineTool);
            tools.Add(rectangleTool);
            tools.Add(ellipseTool);

            var currentTool = tools.FirstOrDefault(t => t.Name == "Selection");

            // MODELS

            var container = new ShapesContainer()
            {
                Width = 720,
                Height = 630
            };

            var workingContainer = new ShapesContainer();

            var style = new DrawStyle(new DrawColor(255, 0, 255, 0), new DrawColor(80, 0, 255, 0), 2.0, true, true);

            var pointShape = new EllipseShape(new PointShape(-4, -4, null), new PointShape(4, 4, null))
            {
                Style = new DrawStyle(new DrawColor(0, 0, 0, 0), new DrawColor(255, 255, 255, 0), 2.0, false, true)
            };

            // DEMOS

            /*
            var group = new GroupShape();
            group.Shapes.Add(new RectangleShape(new PointShape(30, 30, pointShape), new PointShape(60, 60, pointShape)) { Style = style });
            group.Points.Add(new PointShape(45, 30, pointShape));
            group.Points.Add(new PointShape(45, 60, pointShape));
            group.Points.Add(new PointShape(30, 45, pointShape));
            group.Points.Add(new PointShape(60, 45, pointShape));
            container.Shapes.Add(group);
            */

            // VIEW MODELS

            var renderer = new WpfShapeRenderer();

            var vm = new ShapesContainerViewModel()
            {
                Tools = tools,
                CurrentTool = currentTool,
                Container = container,
                WorkingContainer = workingContainer,
                Style = style,
                PointShape = pointShape,
                Renderer = renderer,
                HitTest = hitTest
            };

            // VIEWS

            var window = new MainView();
            window.DataContext = vm;
            window.ShowDialog();
        }
    }
}
