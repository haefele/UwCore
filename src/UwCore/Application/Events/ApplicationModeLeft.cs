using JetBrains.Annotations;
using UwCore.Common;

namespace UwCore.Application.Events
{
    public class ApplicationModeLeft
    {
        public ApplicationMode ApplicationMode { get; }

        public ApplicationModeLeft([NotNull]ApplicationMode applicationMode)
        {
            Guard.NotNull(applicationMode, nameof(applicationMode));

            this.ApplicationMode = applicationMode;
        }
    }
}