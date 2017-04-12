﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.ObjectModel;
using Draw2D.Core.Shapes;
using Draw2D.Core.Style;

namespace Draw2D.Core.Containers
{
    public class ShapesContainer : NamedObject, IShapesContainer, ICopyable<ShapesContainer>
    {
        private double _width;
        private double _height;
        private ObservableCollection<DrawStyle> _styles;
        private ObservableCollection<LineShape> _guides;
        private ObservableCollection<ShapeObject> _shapes;

        public double Width
        {
            get => _width;
            set => Update(ref _width, value);
        }

        public double Height
        {
            get => _height;
            set => Update(ref _height, value);
        }

        public ObservableCollection<DrawStyle> Styles
        {
            get => _styles;
            set => Update(ref _styles, value);
        }

        public ObservableCollection<LineShape> Guides
        {
            get => _guides;
            set => Update(ref _guides, value);
        }

        public ObservableCollection<ShapeObject> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        public ShapesContainer()
        {
            _styles = new ObservableCollection<DrawStyle>();
            _guides = new ObservableCollection<LineShape>();
            _shapes = new ObservableCollection<ShapeObject>();
        }

        public ShapesContainer Copy()
        {
            return new ShapesContainer()
            {
                Name = this.Name
            };
        }
    }
}
