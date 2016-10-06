using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Loading;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : ReactiveScreen
    {
        private readonly ILoadingService _loadingService;
        private readonly INavigationService _navigationService;

        private readonly ObservableAsPropertyHelper<string> _someUnitHelper;

        public int SomeId { get; set; }

        public string SomeUnit => this._someUnitHelper.Value;

        public UwCoreCommand<string> Test { get; }

        public TestViewModel(ILoadingService loadingService, INavigationService navigationService)
        {
            this._loadingService = loadingService;
            this._navigationService = navigationService;

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
            this.Log();

            await this.Test.ExecuteAsync();
            this.Log();
        }

        private async Task<string> TestImpl(CancellationToken token)
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);

            this.Log();

            return "Holla";
        }
    }
}