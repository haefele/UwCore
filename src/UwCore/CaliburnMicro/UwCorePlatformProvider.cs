using Windows.ApplicationModel;
using UwCore.Extensions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using UwCore.Controls;

namespace Caliburn.Micro
{

    /// <summary>
    /// A <see cref="IPlatformProvider"/> implementation for the XAML platfrom.
    /// </summary>
    internal class UwCorePlatformProvider : IPlatformProvider
    {
        #region Fields
        private readonly CoreDispatcher _dispatcher;
        #endregion

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="UwCorePlatformProvider"/> class.
        /// </summary>
        public UwCorePlatformProvider()
        {
            this._dispatcher = Window.Current.Dispatcher;
        }
        #endregion

        #region Implementation of IPlatformProvider
        /// <summary>
        /// Indicates whether or not the framework is in design-time mode.
        /// </summary>
        bool IPlatformProvider.InDesignMode => DesignMode.DesignModeEnabled;

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        async void IPlatformProvider.BeginOnUIThread(Action action)
        {
            this.ValidateDispatcher();
            await this._dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action());
        }

        /// <summary>
        /// Executes the action on the UI thread asynchronously.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <returns></returns>
        Task IPlatformProvider.OnUIThreadAsync(Action action)
        {
            this.ValidateDispatcher();

            return this._dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask();
        }

        /// <summary>
        /// Executes the action on the UI thread.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        void IPlatformProvider.OnUIThread(Action action)
        {
            if (this.CheckAccess())
            {   
                action();
            }
            else
            {
                this._dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => action()).AsTask().Wait();
            }
        }

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <param name="view">The view to search.</param>
        /// <returns>
        /// The root element that was not created by the framework.
        /// </returns>
        /// <remarks>
        /// In certain instances the services create UI elements.
        /// For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the UserControl in.
        /// The WindowManager marks that element as a framework-created element so that it can determine what it created vs. what was intended by the developer.
        /// Calling GetFirstNonGeneratedView allows the framework to discover what the original element was.
        /// </remarks>
        object IPlatformProvider.GetFirstNonGeneratedView(object view)
        {
            return View.GetFirstNonGeneratedView(view);
        }

        /// <summary>
        /// Executes the handler the fist time the view is loaded.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        void IPlatformProvider.ExecuteOnFirstLoad(object view, Action<object> handler)
        {
            var frameworkElement = view as FrameworkElement;

            if (frameworkElement == null)
                return;

            if (frameworkElement.IsLoaded())
            {
                handler(frameworkElement);
                return;
            }

            RoutedEventHandler loaded = null;
            loaded = (s, e) => 
            {
                frameworkElement.Loaded -= loaded;
                handler(s);
            };

            frameworkElement.Loaded += loaded;
        }

        /// <summary>
        /// Executes the handler the next time the view's LayoutUpdated event fires.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="handler">The handler.</param>
        void IPlatformProvider.ExecuteOnLayoutUpdated(object view, Action<object> handler)
        {
            var frameworkElement = view as FrameworkElement;

            if (frameworkElement == null)
                return;

            EventHandler<object> onLayoutUpdate = null;
            onLayoutUpdate = (s, e) => 
            {
                frameworkElement.LayoutUpdated -= onLayoutUpdate;
                handler(frameworkElement);
            };

            frameworkElement.LayoutUpdated += onLayoutUpdate;
        }

        /// <summary>
        /// Get the close action for the specified view model.
        /// </summary>
        /// <param name="viewModel">The view model to close.</param>
        /// <param name="views">The associated views.</param>
        /// <param name="dialogResult">The dialog result.</param>
        /// <returns>
        /// An <see cref="Action" /> to close the view model.
        /// </returns>
        /// <exception cref="System.NotImplementedException"></exception>
        Action IPlatformProvider.GetViewCloseAction(object viewModel, ICollection<object> views, bool? dialogResult)
        {
            var child = viewModel as IChild;

            var conductor = child?.Parent as IConductor;
            if (conductor != null)
            {
                return () => conductor.CloseItem(viewModel);
            }

            foreach (var contextualView in views)
            {
                var viewType = contextualView.GetType();
                var closeMethod = viewType.GetRuntimeMethod("Close", new Type[0]);

                if (closeMethod != null)
                    return () => 
                    {
                        closeMethod.Invoke(contextualView, null);
                    };
                
                var isOpenProperty = viewType.GetRuntimeProperty("IsOpen");
                if (isOpenProperty != null)
                {
                    return () => isOpenProperty.SetValue(contextualView, false, null);
                }

                var parentPopup = (contextualView as FrameworkElement).FindAscendant<PopupOverlay>();
                if (parentPopup != null)
                {
                    return () => parentPopup.Close();
                }
            }

            return () => LogManager.GetLog(typeof(Screen)).Info("TryClose requires a parent IConductor or a view with a Close method or IsOpen property.");
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Makes sure that there is a <see cref="_dispatcher"/>.
        /// </summary>
        private void ValidateDispatcher()
        {
            if (this._dispatcher == null)
                throw new InvalidOperationException("Not initialized with dispatcher.");
        }
        /// <summary>
        /// Checks
        /// </summary>
        private bool CheckAccess()
        {
            return this._dispatcher == null || Window.Current != null;
        }
        #endregion
    }
}
