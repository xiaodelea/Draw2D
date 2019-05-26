﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Draw2D.Views;

namespace Draw2D
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            if (AppState.ParseArgs(args) == true)
            {
                BuildAvaloniaApp().Start(AppMain, args);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .With(new Win32PlatformOptions { AllowEglInitialization = false })
                         .With(new X11PlatformOptions { UseGpu = true, UseEGL = true })
                         .With(new AvaloniaNativePlatformOptions { UseGpu = true })
                         .UseSkia()
                         .LogToDebug();

        private static void AppMain(Application app, string[] args)
        {
            AppState.Load();

            var window = new MainWindow
            {
                DataContext = AppState.ToolContext
            };

            AppState.SetWindowSettings(window);

            window.Closing += (sender, e) =>
            {
                AppState.GetWindowSettings(window);
            };

            app.Run(window);

            AppState.Save();
        }
    }
}
