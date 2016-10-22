using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore;
using UwCore.Application;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.ApplicationState;
using UwCore.Services.Loading;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : ReactiveScreen
    {
        private readonly ILoadingService _loadingService;
        private readonly INavigationService _navigationService;
        private readonly IApplication _application;

        private readonly ObservableAsPropertyHelper<string> _someUnitHelper;

        public int SomeId { get; set; }

        public string SomeUnit => this._someUnitHelper.Value;

        public UwCoreCommand<string> Test { get; }

        public TestViewModel(ILoadingService loadingService, INavigationService navigationService, IApplication application)
        {
            this._loadingService = loadingService;
            this._navigationService = navigationService;
            this._application = application;

            this.DisplayName = "Statistics from 9/1/2016 to 9/30/2016";
            
            this.Test = UwCoreCommand.Create(this.TestImpl)
                .ShowLoadingOverlay("Test-Message")
                .HandleExceptions()
                .TrackEvent("TestCommand");
            
            this.Test.ToProperty(this, f => f.SomeUnit, out this._someUnitHelper);
        }
        
        private void Log()
        {
            LoggerFactory.GetLogger<TestViewModel>().Debug($"IsExecuting: {this.Test.IsExecuting}, CanExecute: {this.Test.CanExecute}");
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

            if (this._application.Theme == ElementTheme.Default)
            {
                this._application.Theme = ElementTheme.Dark;
            }
            else if (this._application.Theme == ElementTheme.Dark)
            {
                this._application.Theme = ElementTheme.Light;
            }
            else if (this._application.Theme == ElementTheme.Light)
            {
                this._application.Theme = ElementTheme.Default;
            }

            return string.Empty;
        }
    }
}