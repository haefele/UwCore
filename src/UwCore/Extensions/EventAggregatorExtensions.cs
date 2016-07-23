using Caliburn.Micro;
using UwCore.Common;

namespace UwCore.Extensions
{
    public static class EventAggregatorExtensions
    {
        public static void SubscribeScreen(this IEventAggregator self, IScreen screen)
        {
            Guard.NotNull(self, nameof(self));
            Guard.NotNull(screen, nameof(screen));

            screen.Activated += (_, __) => self.Subscribe(screen);
            screen.Deactivated += (_, __) => self.Unsubscribe(screen);
        }
    }
}