using System;
using UwCore.Common;
using UwCore.Controls;

namespace UwCore.Services.Loading
{
    public class LoadingService : ILoadingService
    {
        private readonly LoadingOverlay _overlay;

        public LoadingService(LoadingOverlay overlay)
        {
            this._overlay = overlay;
        }

        public IDisposable Show(string message)
        {
            if (this._overlay.IsActive)
                return new DisposableAction(() => { });

            this._overlay.Message = message;
            this._overlay.IsActive = true;

            return new DisposableAction(() =>
            {
                this._overlay.IsActive = false;
            });
        }
    }
}