using System;
using UwCore.Common;
using UwCore.Controls;

namespace UwCore.Services.Loading
{
    public class LoadingService : ILoadingService
    {
        private readonly LoadingOverlay _overlayOld;

        public LoadingService(LoadingOverlay overlayOld)
        {
            this._overlayOld = overlayOld;
        }

        public IDisposable Show(string message)
        {
            if (this._overlayOld.IsActive)
                return new DisposableAction(() => { });

            this._overlayOld.Message = message;
            this._overlayOld.IsActive = true;

            return new DisposableAction(() =>
            {
                this._overlayOld.IsActive = false;
            });
        }
    }
}