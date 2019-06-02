﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;

namespace Draw2D.Input
{
    [Flags]
    public enum Modifier
    {
        None = 0,
        Alt = 1,
        Control = 2,
        Shift = 4
    }
}
