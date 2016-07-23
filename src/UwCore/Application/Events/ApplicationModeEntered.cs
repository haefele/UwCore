using UwCore.Common;

namespace UwCore.Application.Events
{
    public class ApplicationModeEntered
    {
        public ApplicationMode ApplicationMode { get; }

        public ApplicationModeEntered(ApplicationMode applicationMode)
        {
            Guard.NotNull(applicationMode, nameof(applicationMode));

            this.ApplicationMode = applicationMode;
        }
    }
}