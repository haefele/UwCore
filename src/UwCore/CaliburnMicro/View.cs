using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Caliburn.Micro
{

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View {
        private static readonly ILog Log = LogManager.GetLog(typeof(View));

        /// <summary>
        /// A dependency property which allows the framework to track whether a certain element has already been loaded in certain scenarios.
        /// </summary>
        public static readonly DependencyProperty IsLoadedProperty =
            DependencyProperty.RegisterAttached(
                "IsLoaded",
                typeof(bool),
                typeof(View),
                new PropertyMetadata(false)
                );
        
        /// <summary>
        /// Used by the framework to indicate that this element was generated.
        /// </summary>
        public static readonly DependencyProperty IsGeneratedProperty =
            DependencyProperty.RegisterAttached(
                "IsGenerated",
                typeof(bool),
                typeof(View),
                new PropertyMetadata(false)
                );

        /// <summary>
        /// Executes the handler immediately if the element is loaded, otherwise wires it to the Loaded event.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        /// <returns>true if the handler was executed immediately; false otherwise</returns>
        public static bool ExecuteOnLoad(FrameworkElement element, RoutedEventHandler handler)
        {
            if (IsElementLoaded(element))
            {
                handler(element, new RoutedEventArgs());
                return true;
            }

            RoutedEventHandler loaded = null;
            loaded = (s, e) => {
                element.Loaded -= loaded;
                handler(s, e);
            };
            element.Loaded += loaded;
            return false;
        }
        
        /// <summary>
        /// Determines whether the specified <paramref name="element"/> is loaded.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>true if the element is loaded; otherwise, false. </returns>
        public static bool IsElementLoaded(FrameworkElement element) {
            try
            {
                if ((element.Parent ?? VisualTreeHelper.GetParent(element)) != null)
                {
                    return true;
                }

                var rootVisual = Window.Current.Content;

                if (rootVisual != null)
                {
                    return element == rootVisual;
                }

                return false;

            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Executes the handler the next time the elements's LayoutUpdated event fires.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="handler">The handler.</param>
        public static void ExecuteOnLayoutUpdated(FrameworkElement element, EventHandler<object> handler) {
            EventHandler<object> onLayoutUpdate = null;
            onLayoutUpdate = (s, e) => {
                element.LayoutUpdated -= onLayoutUpdate;
                handler(element, e);
            };
            element.LayoutUpdated += onLayoutUpdate;
        }

        /// <summary>
        /// Used to retrieve the root, non-framework-created view.
        /// </summary>
        /// <param name="view">The view to search.</param>
        /// <returns>The root element that was not created by the framework.</returns>
        /// <remarks>In certain instances the services create UI elements.
        /// For example, if you ask the window manager to show a UserControl as a dialog, it creates a window to host the UserControl in.
        /// The WindowManager marks that element as a framework-created element so that it can determine what it created vs. what was intended by the developer.
        /// Calling GetFirstNonGeneratedView allows the framework to discover what the original element was. 
        /// </remarks>
        public static Func<object, object> GetFirstNonGeneratedView = view => {
            var dependencyObject = view as DependencyObject;
            if (dependencyObject == null)
            {
                return view;
            }

            if ((bool)dependencyObject.GetValue(IsGeneratedProperty))
            {
                if (dependencyObject is ContentControl)
                {
                    return ((ContentControl)dependencyObject).Content;
                }

                var type = dependencyObject.GetType();
                var contentPropertyName = GetContentPropertyName(type);

                return type.GetRuntimeProperty(contentPropertyName)
                    .GetValue(dependencyObject, null);
            }

            return dependencyObject;
        };

        #region Model Attached Property
        /// <summary>
        /// A dependency property for attaching a model to the UI.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.RegisterAttached(
            "Model", 
            typeof(object), 
            typeof(View), 
            new PropertyMetadata(null, OnModelChanged));

        /// <summary>
        /// Gets the model.
        /// </summary>
        /// <param name="d">The element the model is attached to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject d)
        {
            return d.GetValue(ModelProperty);
        }
        /// <summary>
        /// Sets the model.
        /// </summary>
        /// <param name="d">The element to attach the model to.</param>
        /// <param name="value">The model.</param>
        public static void SetModel(DependencyObject d, object value)
        {
            d.SetValue(ModelProperty, value);
        }
        
        /// <summary>
        /// Executed when the <see cref="ModelProperty"/> changed.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> whose <see cref="ModelProperty"/> changed.</param>
        /// <param name="e">The event args.</param>
        private static void OnModelChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            UpdateViewModel(dependencyObject);
        }
        #endregion

        #region Context Attached Property
        /// <summary>
        /// A dependency property for assigning a context to a particular portion of the UI.
        /// </summary>
        public static readonly DependencyProperty ContextProperty = DependencyProperty.RegisterAttached(
            "Context",
            typeof(object),
            typeof(View),
            new PropertyMetadata(null, OnContextChanged));

        /// <summary>
        /// Gets the context.
        /// </summary>
        /// <param name="dependencyObject">The element the context is attached to.</param>
        /// <returns>The context.</returns>
        public static object GetContext(DependencyObject dependencyObject)
        {
            return dependencyObject.GetValue(ContextProperty);
        }
        /// <summary>
        /// Sets the context.
        /// </summary>
        /// <param name="dependencyObject">The element to attach the context to.</param>
        /// <param name="value">The context.</param>
        public static void SetContext(DependencyObject dependencyObject, object value)
        {
            dependencyObject.SetValue(ContextProperty, value);
        }

        /// <summary>
        /// Executed when the <see cref="ContextProperty"/> changed.
        /// </summary>
        /// <param name="dependencyObject">The <see cref="DependencyObject"/> whose <see cref="ContextProperty"/> was changed.</param>
        /// <param name="e">The event args.</param>
        private static void OnContextChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue == e.NewValue)
                return;

            UpdateViewModel(dependencyObject);
        }
        #endregion

        #region Model and Context Methods
        private static void UpdateViewModel(DependencyObject dependencyObject)
        {
            var model = GetModel(dependencyObject);
            var context = GetContext(dependencyObject);

            var view = ViewLocator.LocateForModel(model, dependencyObject, context);
            SetContentProperty(dependencyObject, view);

            ViewModelBinder.Bind(model, view, context);
        }
        #endregion

        #region Content Property
        private static bool SetContentProperty(object targetLocation, object view)
        {
            Func<object, object, bool> setContentCore = (object currentTargetLocation, object currentView) =>
            {
                try
                {
                    var type = currentTargetLocation.GetType();
                    var contentPropertyName = GetContentPropertyName(type);

                    type.GetRuntimeProperty(contentPropertyName)
                        .SetValue(currentTargetLocation, currentView, null);

                    return true;
                }
                catch (Exception e)
                {
                    Log.Error(e);

                    return false;
                }
            };

            var frameworkElement = view as FrameworkElement;
            if (frameworkElement?.Parent != null)
            {
                //First remove it from it's current parent
                setContentCore(frameworkElement.Parent, null);
            }

            //Then add it to it's new parent
            return setContentCore(targetLocation, view);
        }
        
        private static string GetContentPropertyName(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            var contentProperty = typeInfo.GetCustomAttribute<ContentPropertyAttribute>();

            return contentProperty == null 
                ? "Content" 
                : contentProperty.Name;
        }
        #endregion
    }
}
