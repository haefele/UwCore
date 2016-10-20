﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Caliburn.Micro;
using UwCore.Application;
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

        public override IEnumerable<Type> GetApplicationModeTypes()
        {
            yield return typeof(NormalApplicationMode);
        }

        public override IEnumerable<Type> GetServiceTypes()
        {
            yield return typeof(IMyService);
            yield return typeof(MyService);
            yield return typeof(MyService);
        }

        public override ApplicationMode GetCurrentMode()
        {
            return IoC.Get<NormalApplicationMode>();
        }

        public override void CustomizeApplication(IApplication application)
        {
            base.CustomizeApplication(application);
            
            application.HeaderDetailsViewModel = IoC.Get<HeaderDetailsViewModel>();
        }
        
        public override void CustomConfiguration()
        {
            ApplicationView.GetForCurrentView().SetPreferredMinSize(new Size(360, 500));
        }

        public override string GetErrorTitle() => "Fehler";

        public override string GetErrorMessage() => "Ups, es ist ein Fehler aufgetreten.";

        public override Type GetCommonExceptionType() => typeof(Exception);
    }
}
