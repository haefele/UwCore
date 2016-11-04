using Windows.UI.Xaml;

namespace Caliburn.Micro
{

    /// <summary>
    /// Binds a view to a view model.
    /// </summary>
    public static class ViewModelBinder
    {

        private static readonly ILog Log = LogManager.GetLog(typeof(ViewModelBinder));

        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        ///<remarks>Passes the the view model, view and creation context (or null for default) to use in applying binding.</remarks>
        public static void Bind(object viewModel, DependencyObject view, object context)
        {
            Log.Info("Binding {0} and {1}.", view, viewModel);

            var viewAware = viewModel as IViewAware;
            if (viewAware != null)
            {
                Log.Info("Attaching {0} to {1}.", view, viewAware);
                viewAware.AttachView(view, context);
            }

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.DataContext = viewModel;
            }
        }
    }
}
