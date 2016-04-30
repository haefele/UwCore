using System;
using System.Threading.Tasks;
using Caliburn.Micro;
using UwCore.Services.Loading;

namespace UwCoreTest.Views.Test
{
    public class TestViewModel : Screen
    {
        private readonly ILoadingService _loadingService;

        public TestViewModel(ILoadingService loadingService)
        {
            this._loadingService = loadingService;
            this.DisplayName = "Test-View-Model";
        }

        public async void Click()
        {
            using (this._loadingService.Show("Test-Message"))
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }
        }
    }
}