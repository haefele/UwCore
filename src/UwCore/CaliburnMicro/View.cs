using System;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using UwCore.Extensions;

namespace Caliburn.Micro
{

    /// <summary>
    /// Hosts attached properties related to view models.
    /// </summary>
    public static class View
    {
        #region Log
        private static readonly ILog Log = LogManager.GetLog(typeof(View));
        #endregion

        #region IsGenerated Attached Property
        /// <summary>
        /// Used by the framework to indicate that this element was generated.
        /// </summary>
        public static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached(
            "IsGenerated", 
            typeof(bool), 
            typeof(View), 
            new PropertyMetadata(false));

        public static void SetIsGenerated(DependencyObject element, bool value)
        {
            element.SetValue(IsGeneratedProperty, value);
        }
        public static bool GetIsGenerated(DependencyObject element)
        {
            return (bool) element.GetValue(IsGeneratedProperty);
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
        public static object GetFirstNonGeneratedView(object view)
        {
            var dependencyObject = view as DependencyObject;

            if (dependencyObject == null)
                return view;

            if (GetIsGenerated(dependencyObject) == false)
                return dependencyObject;
            
            var contentControl = dependencyObject as ContentControl;
            if (contentControl != null)
            {
                return contentControl.Content;
            }

            var type = dependencyObject.GetType();
            var contentPropertyName = GetContentPropertyName(type);

            return type
                .GetRuntimeProperty(contentPropertyName)
                .GetValue(dependencyObject, null);
        }
        #endregion

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
