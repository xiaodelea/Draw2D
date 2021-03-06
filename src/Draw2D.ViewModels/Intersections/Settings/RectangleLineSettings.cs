﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Runtime.Serialization;

namespace Draw2D.ViewModels.Intersections
{
    [DataContract(IsReference = true)]
    public class RectangleLineSettings : SettingsBase
    {
        private bool _isEnabled;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsEnabled
        {
            get => _isEnabled;
            set => Update(ref _isEnabled, value);
        }
    }
}
