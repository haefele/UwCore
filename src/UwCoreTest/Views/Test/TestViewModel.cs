using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore;
using UwCore.Extensions;
using UwCore.Services.Loading;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : ReactiveScreen
    {
        private readonly ILoadingService _loadingService;
        private readonly INavigationService _navigationService;
        private ObservableAsPropertyHelper<Unit> _someUnitHelper;

        public int SomeId { get; set; }

        public Unit SomeUnit => this._someUnitHelper.Value;

        public UwCoreCommand<Unit> Test { get; }

        public TestViewModel(ILoadingService loadingService, INavigationService navigationService)
        {
            this._loadingService = loadingService;
            this._navigationService = navigationService;

            this.DisplayName = "Statistics from 9/1/2016 to 9/30/2016";

            this.Test = UwCoreCommand.Create(this.TestImpl)
                .ShowLoadingService("Test-Message")
                .HandleExceptions()
                .TrackEvent("TestCommand");

            this.Test.ToProperty(this, f => f.SomeUnit, out this._someUnitHelper);
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public override void CanClose(Action<bool> callback)
        {
            base.CanClose(callback);
        }

        private static bool _asPopup = true;

        public async Task TestImpl()
        {
            throw new Exception("Bääm bääm bääm bääm");
        }
    }
}