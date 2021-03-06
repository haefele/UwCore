﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Caliburn.Micro;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.ApplicationState;
using UwCore.Services.Loading;
using UwCore.Services.Navigation;

namespace UwCoreTest.Views.Test
{
    public class TestViewModelParams
    {
        public int SomeId { get; set; }
    }

    public class TestViewModel : UwCoreScreen
    {

        private readonly ILoadingService _loadingService;
        private readonly INavigationService _navigationService;
        private readonly IShell _shell;

        private readonly ObservableAsPropertyHelper<string> _someUnitHelper;
        
        public string SomeUnit => this._someUnitHelper.Value;

        public TestViewModelParams Parameters { get; set; }

        public UwCoreCommand<string> Test { get; }

        public TestViewModel(ILoadingService loadingService, INavigationService navigationService, IShell shell)
        {
            this._loadingService = loadingService;
            this._navigationService = navigationService;
            this._shell = shell;

            this.DisplayName = "Statistics from 9/1/2016 to 9/30/2016";
            
            this.Test = UwCoreCommand.Create(this.TestImpl)
                .ShowLoadingOverlay("Test-Message")
                .HandleExceptions()
                .TrackEvent("TestCommand");
            
            this.Test.ToProperty(this, f => f.SomeUnit, out this._someUnitHelper);
        }
        
        private void Log()
        {
            LogManager.GetLog(typeof(TestViewModel)).Info($"IsExecuting: {this.Test.IsExecuting}, CanExecute: {this.Test.CanExecute}");
        }

        protected override void RestoreState(IApplicationStateService applicationStateService)
        {
            base.RestoreState(applicationStateService);

            int thingy = applicationStateService.Get<int>("Thingy", ApplicationState.Local);
        }

        protected override void SaveState(IApplicationStateService applicationStateService)
        {
            base.SaveState(applicationStateService);

            applicationStateService.Set("Thingy", 123, ApplicationState.Local);
        }

        protected override async void OnActivate()
        {
            var stateService = IoC.Get<IApplicationStateService>();

            var myStateService = stateService
                .GetStateServiceFor(this.GetType())
                .GetStateServiceFor(typeof(string));

            myStateService.Set("Holla", "Yuhu", ApplicationState.Local);
            var value = myStateService.Get<string>("Holla", ApplicationState.Local);
        }

        private async Task<string> TestImpl(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);

            //if (this._shell.Theme == ElementTheme.Default)
            //{
            //    this._shell.Theme = ElementTheme.Dark;
            //}
            //else if (this._shell.Theme == ElementTheme.Dark)
            //{
            //    this._shell.Theme = ElementTheme.Light;
            //}
            //else if (this._shell.Theme == ElementTheme.Light)
            //{
            //    this._shell.Theme = ElementTheme.Default;
            //}

            return string.Empty;
        }
    }
}