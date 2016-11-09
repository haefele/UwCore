using UwCore.Common;

namespace UwCore.Application.Events
{
    public class ShellModeEntered
    {
        public ShellMode ShellMode { get; }

        public ShellModeEntered(ShellMode shellMode)
        {
            Guard.NotNull(shellMode, nameof(shellMode));

            this.ShellMode = shellMode;
        }
    }
}