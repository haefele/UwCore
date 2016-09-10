using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using UwCore.Application;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.ApplicationState;

namespace UwCore.Services.UpdateNotes
{
    public class UpdateNotesService : IUpdateNotesService
    {
        private readonly IApplicationStateService _applicationStateService;

        public UpdateNotesService(IApplicationStateService applicationStateService)
        {
            Guard.NotNull(applicationStateService, nameof(applicationStateService));

            this._applicationStateService = applicationStateService;
        }

        public bool HasSeenUpdateNotes()
        {
            var currentVersion = Package.Current.Id.Version.ToVersion();
            var seenVersion = this._applicationStateService.Get<Version>("SeenUpdateNotes", ApplicationState.ApplicationState.Local);

            if (seenVersion == null)
                return false;

            return currentVersion == seenVersion;
        }

        public void MarkUpdateNotesAsSeen()
        {
            var currentVersion = Package.Current.Id.Version.ToVersion();
            this._applicationStateService.Set("SeenUpdateNotes", currentVersion, ApplicationState.ApplicationState.Local);
        }

        public void Clear()
        {
            this._applicationStateService.Set("SeenUpdateNotes", (Version)null, ApplicationState.ApplicationState.Local);
        }
    }
}
