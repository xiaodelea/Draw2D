﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Draw2D.ViewModels
{
    public interface ISelectable
    {
        void Move(ISelectionState selectionState, double dx, double dy);
        void Select(ISelectionState selectionState);
        void Deselect(ISelectionState selectionState);
    }
}
