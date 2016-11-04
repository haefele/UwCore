namespace Caliburn.Micro {
    using System;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// A custom IoC container which integrates with WinRT and properly registers all Caliburn.Micro services.
    /// </summary>
    public class WinRTContainer : SimpleContainer {
        /// <summary>
        /// Registers the Caliburn.Micro WinRT services with the container.
        /// </summary>
        public void RegisterWinRTServices() {
            RegisterInstance(typeof (SimpleContainer), null, this);
            RegisterInstance(typeof (WinRTContainer), null, this);

            if (!HasHandler(typeof (IEventAggregator), null)) {
                RegisterSingleton(typeof (IEventAggregator), null, typeof (EventAggregator));
            }
        }
    }
}
