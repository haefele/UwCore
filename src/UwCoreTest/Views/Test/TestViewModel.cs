using System;
using System.Reactive;
using System.Threading.Tasks;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
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
        }
        
        protected override void OnActivate()
        {
            base.OnActivate();
        }

        public async Task TestImpl()
        {
            using (this._loadingService.Show("Test-Message" + this.SomeId))
            {
                using (this._loadingService.Show("Inner Loading"))
                {
                    await Task.Delay(TimeSpan.FromSeconds(2));
                }

                await Task.Delay(TimeSpan.FromSeconds(3));
            }
        }
    }
}