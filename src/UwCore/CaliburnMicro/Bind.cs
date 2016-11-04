using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

#if XFORMS
namespace Caliburn.Micro.Xamarin.Forms
#else
namespace Caliburn.Micro
#endif 
{
    using System;
#if WinRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#elif XFORMS
    using global::Xamarin.Forms;
    using UIElement = global::Xamarin.Forms.Element;
    using FrameworkElement = global::Xamarin.Forms.VisualElement;
    using DependencyProperty = global::Xamarin.Forms.BindableProperty;
    using DependencyObject =global::Xamarin.Forms.BindableObject;
#else
    using System.Windows;
    using System.Windows.Data;
#endif

    public class MessageBinder
    {

        /// <summary>
        /// Custom converters used by the framework registered by destination type for which they will be selected.
        /// The converter is passed the existing value to convert and a "context" object.
        /// </summary>
        public static readonly Dictionary<Type, Func<object, object, object>> CustomConverters =
            new Dictionary<Type, Func<object, object, object>>
            {
                {
                    typeof (DateTime), (value, context) => {
                        DateTime result;
                        DateTime.TryParse(value.ToString(), out result);
                        return result;
                    }
                }
            };

        /// <summary>
        /// Coerces the provided value to the destination type.
        /// </summary>
        /// <param name="destinationType">The destination type.</param>
        /// <param name="providedValue">The provided value.</param>
        /// <param name="context">An optional context value which can be used during conversion.</param>
        /// <returns>The coerced value.</returns>
        public static object CoerceValue(Type destinationType, object providedValue, object context)
        {
            if (providedValue == null)
            {
                return GetDefaultValue(destinationType);
            }

            var providedType = providedValue.GetType();
            if (destinationType.IsAssignableFrom(providedType))
            {
                return providedValue;
            }

            if (CustomConverters.ContainsKey(destinationType))
            {
                return CustomConverters[destinationType](providedValue, context);
            }

            try
            {
#if !WinRT && !XFORMS
                var converter = TypeDescriptor.GetConverter(destinationType);

                if (converter.CanConvertFrom(providedType)) {
                    return converter.ConvertFrom(providedValue);
                }

                converter = TypeDescriptor.GetConverter(providedType);

                if (converter.CanConvertTo(destinationType)) {
                    return converter.ConvertTo(providedValue, destinationType);
                }
#endif
#if WinRT || XFORMS
                if (destinationType.GetTypeInfo().IsEnum)
                {
#else
                if (destinationType.IsEnum) {
#endif
                    var stringValue = providedValue as string;
                    if (stringValue != null)
                    {
                        return Enum.Parse(destinationType, stringValue, true);
                    }

                    return Enum.ToObject(destinationType, providedValue);
                }

                if (typeof(Guid).IsAssignableFrom(destinationType))
                {
                    var stringValue = providedValue as string;
                    if (stringValue != null)
                    {
                        return new Guid(stringValue);
                    }
                }
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }

            try
            {
                return Convert.ChangeType(providedValue, destinationType, CultureInfo.CurrentCulture);
            }
            catch
            {
                return GetDefaultValue(destinationType);
            }
        }


        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The default value.</returns>
        public static object GetDefaultValue(Type type)
        {
#if WinRT || XFORMS
            var typeInfo = type.GetTypeInfo();
            return typeInfo.IsClass || typeInfo.IsInterface ? null : System.Activator.CreateInstance(type);
#else
            return type.IsClass || type.IsInterface ? null : Activator.CreateInstance(type);
#endif
        }
    }

    /// <summary>
    ///   Hosts dependency properties for binding.
    /// </summary>
    public static class Bind {
        /// <summary>
        ///   Allows binding on an existing view. Use this on root UserControls, Pages and Windows; not in a DataTemplate.
        /// </summary>
        public static DependencyProperty ModelProperty =
            DependencyPropertyHelper.RegisterAttached(
                "Model",
                typeof(object),
                typeof(Bind),
                null, 
                ModelChanged);

        /// <summary>
        ///   Allows binding on an existing view without setting the data context. Use this from within a DataTemplate.
        /// </summary>
        public static DependencyProperty ModelWithoutContextProperty =
            DependencyPropertyHelper.RegisterAttached(
                "ModelWithoutContext",
                typeof(object),
                typeof(Bind),
                null, 
                ModelWithoutContextChanged);

        internal static DependencyProperty NoContextProperty =
            DependencyPropertyHelper.RegisterAttached(
                "NoContext",
                typeof(bool),
                typeof(Bind),
                false);

        /// <summary>
        ///   Gets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <returns>The model.</returns>
        public static object GetModelWithoutContext(DependencyObject dependencyObject) {
            return dependencyObject.GetValue(ModelWithoutContextProperty);
        }

        /// <summary>
        ///   Sets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <param name = "value">The model.</param>
        public static void SetModelWithoutContext(DependencyObject dependencyObject, object value) {
            dependencyObject.SetValue(ModelWithoutContextProperty, value);
        }

        /// <summary>
        ///   Gets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <returns>The model.</returns>
        public static object GetModel(DependencyObject dependencyObject) {
            return dependencyObject.GetValue(ModelProperty);
        }

        /// <summary>
        ///   Sets the model to bind to.
        /// </summary>
        /// <param name = "dependencyObject">The dependency object to bind to.</param>
        /// <param name = "value">The model.</param>
        public static void SetModel(DependencyObject dependencyObject, object value) {
            dependencyObject.SetValue(ModelProperty, value);
        }

        static void ModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (View.InDesignMode || e.NewValue == null || e.NewValue == e.OldValue) {
                return;
            }

            var fe = d as FrameworkElement;
            if (fe == null) {
                return;
            }

            View.ExecuteOnLoad(fe, delegate {
                var target = e.NewValue;

                d.SetValue(View.IsScopeRootProperty, true);

#if XFORMS
                var context = fe.Id.ToString("N");
#else
                var context = string.IsNullOrEmpty(fe.Name)
                                  ? fe.GetHashCode().ToString()
                                  : fe.Name;
#endif

                ViewModelBinder.Bind(target, d, context);
            });
        }

        static void ModelWithoutContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (View.InDesignMode || e.NewValue == null || e.NewValue == e.OldValue) {
                return;
            }

            var fe = d as FrameworkElement;
            if (fe == null) {
                return;
            }

            View.ExecuteOnLoad(fe, delegate {
                var target = e.NewValue;
                var containerKey = e.NewValue as string;
                if (containerKey != null) {
                    LogManager.GetLog(typeof(Bind)).Info("Using IoC is deprecated and will be removed in v3.0");
                    target = IoC.GetInstance(null, containerKey);
                }

                d.SetValue(View.IsScopeRootProperty, true);

#if XFORMS
                var context = fe.Id.ToString("N");
#else
                var context = string.IsNullOrEmpty(fe.Name)
                                  ? fe.GetHashCode().ToString()
                                  : fe.Name;
#endif

                d.SetValue(NoContextProperty, true);
                ViewModelBinder.Bind(target, d, context);
            });
        }

        /// <summary>
        /// Allows application of conventions at design-time.
        /// </summary>
        public static DependencyProperty AtDesignTimeProperty =
            DependencyPropertyHelper.RegisterAttached(
                "AtDesignTime",
                typeof(bool),
                typeof(Bind),
                false, 
                AtDesignTimeChanged);

        /// <summary>
        /// Gets whether or not conventions are being applied at design-time.
        /// </summary>
        /// <param name="dependencyObject">The ui to apply conventions to.</param>
        /// <returns>Whether or not conventions are applied.</returns>
#if NET
        [AttachedPropertyBrowsableForTypeAttribute(typeof(DependencyObject))]
#endif
        public static bool GetAtDesignTime(DependencyObject dependencyObject) {
            return (bool)dependencyObject.GetValue(AtDesignTimeProperty);
        }

        /// <summary>
        /// Sets whether or not do bind conventions at design-time.
        /// </summary>
        /// <param name="dependencyObject">The ui to apply conventions to.</param>
        /// <param name="value">Whether or not to apply conventions.</param>
        public static void SetAtDesignTime(DependencyObject dependencyObject, bool value) {
            dependencyObject.SetValue(AtDesignTimeProperty, value);
        }

        static void AtDesignTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!View.InDesignMode)
                return;

            var atDesignTime = (bool) e.NewValue;
            if (!atDesignTime)
                return;
#if XFORMS
            d.SetBinding(DataContextProperty, String.Empty);
#else
            BindingOperations.SetBinding(d, DataContextProperty, new Binding());
#endif
        }

        static readonly DependencyProperty DataContextProperty =
            DependencyPropertyHelper.RegisterAttached(
                "DataContext",
                typeof(object),
                typeof(Bind),
                null, DataContextChanged);

        static void DataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            if (!View.InDesignMode)
                return;

            var enable = d.GetValue(AtDesignTimeProperty);
            if (enable == null || ((bool)enable) == false || e.NewValue == null)
                return;

            var fe = d as FrameworkElement;
            if (fe == null)
                return;
#if XFORMS
            ViewModelBinder.Bind(e.NewValue, d, fe.Id.ToString("N"));
#else
            ViewModelBinder.Bind(e.NewValue, d, string.IsNullOrEmpty(fe.Name) ? fe.GetHashCode().ToString() : fe.Name);
#endif
        }
    }
}
