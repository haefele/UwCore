using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore.Extensions;
using UwCore.Services.Loading;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : ReactiveScreen
    {
        private readonly ILoadingService _loadingService;

        public int SomeId { get; set; }

        public ReactiveCommand<Unit> Test { get; }

        public TestViewModel(ILoadingService loadingService)
        {
            this._loadingService = loadingService;

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

        public async Task TestImpl()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));
            await Task.Delay(TimeSpan.FromSeconds(3));

            throw new Exception("Holla");
        }
    }
}