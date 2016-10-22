using System;
using Caliburn.Micro.ReactiveUI;

namespace UwCoreTest.Views.MahPopup
{
    public class MahPopupViewModel : ReactiveScreen
    {
        public override void CanClose(Action<bool> callback)
        {
            callback(false);
        }
    }
}