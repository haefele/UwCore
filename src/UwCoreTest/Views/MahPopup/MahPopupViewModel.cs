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
                .Create(_ => Task.Delay(TimeSpan.FromSeconds(2)))
                .ShowLoadingOverlay("asdflöjkasdfölkj asdf");
        }
    }
}