﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using Microsoft.AppCenter.Analytics;
using UwCore.Application;
using UwCore.Services.Analytics;
using UwCoreTest.ApplicationModes;
using UwCoreTest.Views.HeaderDetails;
using UwCoreTest.Views.MahPopup;
using UwCoreTest.Views.Test;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCoreTest
{
    public interface IMyService
    {
        
    }

    public class MyService : IMyService
    {
        
    }

    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App
    {
        public App()
        {
            this.InitializeComponent();
        }

        public override IEnumerable<Type> GetViewModelTypes()
        {
            yield return typeof(TestViewModel);
            yield return typeof(HeaderDetailsViewModel);
            yield return typeof(MahPopupViewModel);
        }

        public override IEnumerable<Type> GetShellModeTypes()
        {
            yield return typeof(NormalShellMode);
        }

        public override IEnumerable<Type> GetServiceTypes()
        {
            yield return typeof(IMyService);
            yield return typeof(MyService);
        }

        public override ShellMode GetCurrentMode()
        {
            return IoC.Get<NormalShellMode>();
        }

        public override void CustomizeShell(IShell shell)
        {
            base.CustomizeShell(shell);
            
            shell.Theme = ElementTheme.Light;
            shell.HeaderDetailsViewModel = IoC.Get<HeaderDetailsViewModel>();
        }

        public override bool IsAnalyticsServiceEnabled()
        {
            return true;
        }

        public override IAnalyticsService GetAnalyticsService()
        {
            return new AppCenterAnalyticsService("08633b56-6a0b-4568-8b93-b5024a34ef20");
        }

        public override bool UseNewShellIfPossible()
        {
            return true;
        }

        public override void Configure()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }

        public override string GetErrorTitle() => "Fehler";

        public override string GetErrorMessage() => "Ups, es ist ein Fehler aufgetreten.";

        public override Type GetCommonExceptionType() => typeof(Exception);
    }
}
