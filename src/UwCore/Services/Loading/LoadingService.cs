using System;
using System.Collections.Concurrent;
using System.Linq;
using UwCore.Common;
using UwCore.Controls;

namespace UwCore.Services.Loading
{
    public class LoadingService : ILoadingService
    {
        private readonly LoadingOverlay _overlay;
        private readonly ConcurrentDictionary<Guid, string> _messages;
        private readonly object _updateLock;

        public LoadingService(LoadingOverlay overlay)
        {
            this._overlay = overlay;
            this._messages = new ConcurrentDictionary<Guid, string>();
            this._updateLock = new object();
        }

        public IDisposable Show(string message)
        {
            var guid = Guid.NewGuid();

            this._messages.AddOrUpdate(guid, message, (_, __) => message);
            this.Update();

            return new DisposableAction(() =>
            {
                this._messages.TryRemove(guid, out message);
                this.Update();
            });
        }
        
        private void Update()
        {
            lock (this._updateLock)
            {
                string message = string.Join(Environment.NewLine, this._messages.Values);

                this._overlay.Message = message;
                this._overlay.IsActive = string.IsNullOrWhiteSpace(this._overlay.Message) == false;
            }
        }
    }
}