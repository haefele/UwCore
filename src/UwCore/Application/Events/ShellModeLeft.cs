using UwCore.Common;

namespace UwCore.Application.Events
{
    public class ShellModeLeft
    {
        public ShellMode ShellMode { get; }

        public ShellModeLeft(ShellMode shellMode)
        {
            Guard.NotNull(shellMode, nameof(shellMode));

            this.ShellMode = shellMode;
        }
    }
}