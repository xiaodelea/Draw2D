﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Draw2D.Core;
using Draw2D.Core.Containers;
using Draw2D.Core.Editor.Tools;
using Draw2D.Core.ViewModels.Containers;
using Draw2D.Wpf.Utilities;
using Microsoft.Win32;

namespace Draw2D.Wpf.Views
{
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            KeyDown += MainView_KeyDown;
        }

        public void SetSelectionTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Selection").FirstOrDefault();
            }
        }

        public void SetLineTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Line").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "Line").FirstOrDefault();
                }
            }
        }

        public void SetCubicBezierTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "CubicBezier").FirstOrDefault();
                }
            }
        }

        public void SetQuadraticBezierTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
                else
                {
                    vm.CurrentTool = vm.Tools.Where(t => t.Name == "QuadraticBezier").FirstOrDefault();
                }
            }
        }

        public void SetPathTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                vm.CurrentTool = vm.Tools.Where(t => t.Name == "Path").FirstOrDefault();
            }
        }

        public void SetMoveTool()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                if (vm.CurrentTool is PathTool pathTool)
                {
                    pathTool.CleanSubTool(vm);
                    pathTool.CurrentSubTool = pathTool.SubTools.Where(t => t.Name == "Move").FirstOrDefault();
                }
            }
        }

        private void MainView_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        New();
                        break;
                    case Key.O:
                        Open();
                        break;
                    case Key.S:
                        SaveAs();
                        break;
                    case Key.X:
                        Cut();
                        break;
                    case Key.C:
                        Copy();
                        break;
                    case Key.V:
                        Paste();
                        break;
                }
            }
            else if (Keyboard.Modifiers == ModifierKeys.None)
            {
                switch (e.Key)
                {
                    case Key.S:
                        SetSelectionTool();
                        break;
                    case Key.L:
                        SetLineTool();
                        break;
                    case Key.C:
                        SetCubicBezierTool();
                        break;
                    case Key.Q:
                        SetQuadraticBezierTool();
                        break;
                    case Key.H:
                        SetPathTool();
                        break;
                    case Key.M:
                        SetMoveTool();
                        break;
                    case Key.Delete:
                        Delete();
                        break;
                }
            }
        }

        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            New();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            SaveAs();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Windows[0].Close();
        }

        private void EditCut_Click(object sender, RoutedEventArgs e)
        {
            Cut();
        }

        private void EditCopy_Click(object sender, RoutedEventArgs e)
        {
            Copy();
        }

        private void EditPaste_Click(object sender, RoutedEventArgs e)
        {
            Paste();
        }

        private void EditDelete_Click(object sender, RoutedEventArgs e)
        {
            Delete();
        }

        private void New()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                New(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void Open()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    Open(path, vm);
                    RendererView.InvalidateVisual();
                }
            }
        }

        private void SaveAs()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Yaml Files (*.yaml)|*.yaml|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            var result = dlg.ShowDialog();
            if (result == true)
            {
                var path = dlg.FileName;
                if (this.DataContext is ShapesContainerViewModel vm)
                {
                    Save(path, vm);
                }
            }
        }

        private void Cut()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Cut(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void Copy()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Copy(vm);
            }
        }

        private void Paste()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Paste(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void Delete()
        {
            if (this.DataContext is ShapesContainerViewModel vm)
            {
                Delete(vm);
                RendererView.InvalidateVisual();
            }
        }

        private void New(ShapesContainerViewModel vm)
        {
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            var container = new ShapesContainer()
            {
                Width = 720,
                Height = 630
            };
            var workingContainer = new ShapesContainer();
            vm.CurrentContainer = container;
            vm.WorkingContainer = new ShapesContainer();
        }

        private void Open(string path, ShapesContainerViewModel vm)
        {
            var yaml = File.ReadAllText(path);
            var container = YamlDotNetSerializer.FromYaml<ShapesContainer>(yaml, YamlDraw2DTagMappings.TagMappings);
            var workingContainer = new ShapesContainer();
            vm.CurrentTool.Clean(vm);
            vm.Renderer.Selected.Clear();
            vm.CurrentContainer = container;
            vm.WorkingContainer = workingContainer;
        }

        private void Save(string path, ShapesContainerViewModel vm)
        {
            var yaml = YamlDotNetSerializer.ToYaml(vm.CurrentContainer, YamlDraw2DTagMappings.TagMappings);
            File.WriteAllText(path, yaml);
        }

        private void Cut(ShapesContainerViewModel vm)
        {
            Copy(vm);
            Delete(vm);
        }

        private void Copy(ShapesContainerViewModel vm)
        {
            var selected = vm.Renderer.Selected;
            var shapes = new List<ShapeObject>();
            foreach (var shape in selected)
            {
                if (vm.CurrentContainer.Shapes.Contains(shape))
                {
                    shapes.Add(shape);
                }
            }
            var yaml = YamlDotNetSerializer.ToYaml(shapes, YamlDraw2DTagMappings.TagMappings);
            Clipboard.SetText(yaml);
        }

        private void Paste(ShapesContainerViewModel vm)
        {
            if (Clipboard.ContainsText())
            {
                var yaml = Clipboard.GetText();
                var shapes = YamlDotNetSerializer.FromYaml<List<ShapeObject>>(yaml, YamlDraw2DTagMappings.TagMappings);
                // TODO: Update styles and templates using Id.
                vm.Renderer.Selected.Clear();
                foreach (var shape in shapes)
                {
                    vm.CurrentContainer.Shapes.Add(shape);
                    shape.Select(vm.Renderer.Selected);
                }
            }
        }

        private void Delete(ShapesContainerViewModel vm)
        {
            foreach (var shape in vm.Renderer.Selected)
            {
                if (vm.CurrentContainer.Shapes.Contains(shape))
                {
                    vm.CurrentContainer.Shapes.Remove(shape);
                }
            }
            vm.Renderer.Selected.Clear();
        }
    }
}