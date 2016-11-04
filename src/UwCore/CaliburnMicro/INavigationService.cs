using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using UwCore.Extensions;

namespace Caliburn.Micro {
    
    public class FrameAdapter {

        private static readonly ILog Log = LogManager.GetLog(typeof(FrameAdapter));
        private const string FrameStateKey = "FrameState";
        private const string ParameterKey = "ParameterKey";

        private readonly Frame frame;
        private event NavigatingCancelEventHandler ExternalNavigatingHandler = delegate { };
        
        public FrameAdapter(Frame frame) {
            this.frame = frame;

            this.frame.Navigating += OnNavigating;
            this.frame.Navigated += OnNavigated;

            // This could leak memory if we're creating and destorying navigation services regularly.
            // Another unlikely scenario though

            var navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.BackRequested += (s, e) =>
            {
                OnBackRequested(e);

                if (e.Handled)
                    return;

                if (CanGoBack)
                {
                    e.Handled = true;
                    GoBack();
                }
            };
        }
        
        /// <summary>
        ///   Occurs before navigation
        /// </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event args. </param>
        protected virtual void OnNavigating(object sender, NavigatingCancelEventArgs e) {
            ExternalNavigatingHandler(sender, e);

            if (e.Cancel)
                return;

            var view = frame.Content as FrameworkElement;

            if (view == null)
                return;

            var guard = view.DataContext as IGuardClose;

            if (guard != null) {
                var shouldCancel = false;
                var runningAsync = true;
                guard.CanClose(result => { runningAsync = false; shouldCancel = !result; });
                if (runningAsync)
                    throw new NotSupportedException("Async CanClose is not supported.");

                if (shouldCancel) {
                    e.Cancel = true;
                    return;
                }
            }

            var deactivator = view.DataContext as IDeactivate;

            if (deactivator != null) {
                deactivator.Deactivate(CanCloseOnNavigating(sender, e));
            }
        }

        /// <summary>
        ///   Occurs after navigation
        /// </summary>
        /// <param name="sender"> The event sender. </param>
        /// <param name="e"> The event args. </param>
        protected virtual void OnNavigated(object sender, NavigationEventArgs e) {

            if (e.Content == null)
                return;
            
            var view = e.Content as Page;

            if (view == null) {
                throw new ArgumentException("View '" + e.Content.GetType().FullName +
                                            "' should inherit from Page or one of its descendents.");
            }

            BindViewModel(view, e.Parameter);
        }

        /// <summary>
        /// Binds the view model.
        /// </summary>
        /// <param name="view">The view.</param>
        protected virtual void BindViewModel(DependencyObject view, object parameter)
        {
            ViewLocator.InitializeComponent(view);

            var viewModel = ViewModelLocator.LocateForView(view);

            if (viewModel == null)
                return;
            
            this.TryInjectParameters(viewModel, parameter);
            ViewModelBinder.Bind(viewModel, view, null);

            var activator = viewModel as IActivate;
            if (activator != null)
            {
                activator.Activate();
            }
        }

        /// <summary>
        ///   Attempts to inject query string parameters from the view into the view model.
        /// </summary>
        /// <param name="viewModel"> The view model.</param>
        /// <param name="parameter"> The parameter.</param>
        protected virtual void TryInjectParameters(object viewModel, object parameter)
        {
            var dictionaryParameter = parameter as IDictionary<string, object>;
            viewModel.InjectValues(dictionaryParameter);
        }

        /// <summary>
        /// Called to check whether or not to close current instance on navigating.
        /// </summary>
        /// <param name="sender"> The event sender from OnNavigating event. </param>
        /// <param name="e"> The event args from OnNavigating event. </param>
        protected virtual bool CanCloseOnNavigating(object sender, NavigatingCancelEventArgs e) {
            return false;
        }
        
        /// <summary>
        ///   Navigates to the specified content.
        /// </summary>
        /// <param name="sourcePageType"> The <see cref="System.Type" /> to navigate to. </param>
        /// <param name="parameter">The object parameter to pass to the target.</param>
        /// <returns> Whether or not navigation succeeded. </returns>
        public virtual bool Navigate(Type sourcePageType, object parameter) {
            return frame.Navigate(sourcePageType, parameter);
        }
        
        /// <summary>
        ///   Navigates back.
        /// </summary>
        public virtual void GoBack() {
            frame.GoBack();
        }
        
        /// <summary>
        ///   Indicates whether the navigator can navigate back.
        /// </summary>
        public virtual bool CanGoBack {
            get { return frame.CanGoBack; }
        }
        
        /// <summary>
        /// Gets a collection of PageStackEntry instances representing the backward navigation history of the Frame.
        /// </summary>
        public virtual IList<PageStackEntry> BackStack {
            get { return frame.BackStack; }
        }

        /// <summary>
        /// Gets a collection of PageStackEntry instances representing the forward navigation history of the Frame.
        /// </summary>
        public virtual IList<PageStackEntry> ForwardStack {
            get { return frame.ForwardStack; }
        }
        
        /// <summary>
        ///  Occurs when the user presses the hardware Back button. Allows the handlers to cancel the default behavior.
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected virtual void OnBackRequested(BackRequestedEventArgs e)
        {
        }
    }
}
