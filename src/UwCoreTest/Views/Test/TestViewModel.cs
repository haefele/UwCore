using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore.Extensions;
using UwCore.Services.Loading;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : ReactiveScreen
    {
        private readonly ILoadingService _loadingService;
        private readonly INavigationService _navigationService;

        public int SomeId { get; set; }

        public ReactiveCommand<Unit> Test { get; }

        public TestViewModel(ILoadingService loadingService, INavigationService navigationService)
        {
            this._loadingService = loadingService;
            this._navigationService = navigationService;

            this.DisplayName = "Test-View-Model";

            this.Test = ReactiveCommand.CreateAsyncTask(_ => this.TestImpl());
            this.Test.AttachLoadingService("Test-Message");
            this.Test.AttachExceptionHandler();
            this.Test.TrackEvent("TestCommand");
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
            if (_asPopup)
                this._navigationService.Popup.For<TestViewModel>().Navigate();
            else
                this._navigationService.For<TestViewModel>().Navigate();

            _asPopup = !_asPopup;
        }
    }
}