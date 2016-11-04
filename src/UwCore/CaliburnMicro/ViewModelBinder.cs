﻿using System.ServiceModel.Channels;
using Windows.UI.Xaml;

#if XFORMS
namespace Caliburn.Micro.Xamarin.Forms
#else
namespace Caliburn.Micro
#endif
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;
#if XFORMS
    using UIElement = global::Xamarin.Forms.Element;
    using FrameworkElement = global::Xamarin.Forms.VisualElement;
    using DependencyProperty = global::Xamarin.Forms.BindableProperty;
    using DependencyObject = global::Xamarin.Forms.BindableObject;
#elif WinRT81
    using Windows.UI.Xaml;
    using Microsoft.Xaml.Interactivity;
#else
    using System.Windows;
#endif

#if WINDOWS_PHONE
    using Microsoft.Phone.Controls;
#endif

    /// <summary>
    /// Binds a view to a view model.
    /// </summary>
    public static class ViewModelBinder {

        static readonly ILog Log = LogManager.GetLog(typeof(ViewModelBinder));
        
        /// <summary>
        /// Binds the specified viewModel to the view.
        /// </summary>
        ///<remarks>Passes the the view model, view and creation context (or null for default) to use in applying binding.</remarks>
        public static Action<object, DependencyObject, object> Bind = (viewModel, view, context) => {
            Log.Info("Binding {0} and {1}.", view, viewModel);
            
            var viewAware = viewModel as IViewAware;
            if (viewAware != null) {
                Log.Info("Attaching {0} to {1}.", view, viewAware);
                viewAware.AttachView(view, context);
            }

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement != null)
            {
                frameworkElement.DataContext = viewModel;
            }
        };
    }
}
