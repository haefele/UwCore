using JetBrains.Annotations;
using UwCore.Common;

namespace UwCore.Application.Events
{
    public class ApplicationModeEntered
    {
        public ApplicationMode ApplicationMode { get; }

        public ApplicationModeEntered([NotNull]ApplicationMode applicationMode)
        {
            Guard.NotNull(applicationMode, nameof(applicationMode));

            this.ApplicationMode = applicationMode;
        }
    }
}