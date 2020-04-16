using System;
using System.Reactive;
using System.Threading.Tasks;
using UwCore;

namespace UwCoreTest.Views.MahPopup
{
    public class MahPopupViewModel : UwCoreScreen
    {
        public UwCoreCommand<Unit> Loading { get; }

        public MahPopupViewModel()
        {
            this.Loading = UwCoreCommand
                .Create(async _ => await Task.Delay(TimeSpan.FromSeconds(2)))
                .ShowLoadingOverlay("asdflöjkasdfölkj asdf");
        }

        protected override async void OnActivate()
        {
            base.OnActivate();

            //await this.Loading.ExecuteAsync();
        }
    }
}