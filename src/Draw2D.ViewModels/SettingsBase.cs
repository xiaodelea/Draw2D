﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels
{
    [DataContract(IsReference = true)]
    public abstract class SettingsBase : ViewModelBase
    {
    }
}
