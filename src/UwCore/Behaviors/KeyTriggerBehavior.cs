using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace UwCore.Behaviors
{
    [ContentProperty(Name = "Actions")]
    public class KeyTriggerBehavior : Behavior<UIElement>
    {
        #region Properties
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            nameof(Actions),
            typeof(ActionCollection),
            typeof(KeyTriggerBehavior),
            new PropertyMetadata(default(ActionCollection)));

        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection)this.GetValue(ActionsProperty);

                if (actions == null)
                {
                    actions = new ActionCollection();
                    this.SetValue(ActionsProperty, actions);
                }

                return actions;
            }
        }

        public static readonly DependencyProperty OnKeyDownProperty = DependencyProperty.Register(
            nameof(OnKeyDown), 
            typeof(bool), 
            typeof(KeyTriggerBehavior), 
            new PropertyMetadata(default(bool), (s, e) => ((KeyTriggerBehavior)s).OnOnKeyDownChanged((bool)e.OldValue, (bool)e.NewValue)));

        public bool OnKeyDown
        {
            get { return (bool) this.GetValue(OnKeyDownProperty); }
            set { this.SetValue(OnKeyDownProperty, value); }
        }

        public static readonly DependencyProperty KeyProperty = DependencyProperty.Register(
            nameof(Key), 
            typeof(VirtualKey), 
            typeof(KeyTriggerBehavior), 
            new PropertyMetadata(VirtualKey.Enter));

        public VirtualKey Key
        {
            get { return (VirtualKey) this.GetValue(KeyProperty); }
            set { this.SetValue(KeyProperty, value); }
        }
        #endregion

        #region Propeties Changed
        private void OnOnKeyDownChanged(bool oldValue, bool newValue)
        {
            this.UnRegisterEvents(oldValue);
            this.RegisterEvents(newValue);
        }
        #endregion

        #region Overrides of Behavior
        protected override void OnAttached()
        {
            base.OnAttached();

            this.RegisterEvents(this.OnKeyDown);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            this.UnRegisterEvents(this.OnKeyDown);
        }
        #endregion

        #region Private Methods
        private void RegisterEvents(bool onKeyDown)
        {
            if (onKeyDown)
            {
                this.AssociatedObject.KeyDown += this.AssociatedObjectOnKeyDownOrUp;
            }
            else
            {
                this.AssociatedObject.KeyUp += this.AssociatedObjectOnKeyDownOrUp;
            }
        }

        private void UnRegisterEvents(bool onKeyDown)
        {
            if (onKeyDown)
            {
                this.AssociatedObject.KeyDown -= this.AssociatedObjectOnKeyDownOrUp;
            }
            else
            {
                this.AssociatedObject.KeyUp -= this.AssociatedObjectOnKeyDownOrUp;
            }
        }

        private void AssociatedObjectOnKeyDownOrUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
        {
            if (keyRoutedEventArgs.Key == this.Key)
            {
                Interaction.ExecuteActions(this.AssociatedObject, this.Actions, keyRoutedEventArgs);
            }
        }
        #endregion
    }
}