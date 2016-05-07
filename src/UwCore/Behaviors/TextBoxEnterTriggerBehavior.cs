using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Microsoft.Xaml.Interactivity;

namespace UwCore.Behaviors
{
    [ContentProperty(Name = "Actions")]
    public class TextBoxEnterTriggerBehavior : Behavior<TextBox>
    {
        public static readonly DependencyProperty ActionsProperty = DependencyProperty.Register(
            "Actions", 
            typeof(ActionCollection), 
            typeof(TextBoxEnterTriggerBehavior), 
            new PropertyMetadata(default(ActionCollection)));

        public ActionCollection Actions
        {
            get
            {
                var actions = (ActionCollection) this.GetValue(ActionsProperty);

                if (actions == null)
                {
                    actions = new ActionCollection();
                    this.SetValue(ActionsProperty, actions);
                }

                return actions;
            }
        }

        public static readonly DependencyProperty OnKeyDownProperty = DependencyProperty.Register(
            "OnKeyDown", 
            typeof(bool), 
            typeof(TextBoxEnterTriggerBehavior), 
            new PropertyMetadata(default(bool), (s, e) => ((TextBoxEnterTriggerBehavior)s).OnOnKeyDownChanged((bool)e.OldValue, (bool)e.NewValue)));

        public bool OnKeyDown
        {
            get { return (bool)this.GetValue(OnKeyDownProperty); }
            set { this.SetValue(OnKeyDownProperty, value); }
        }

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
            if (keyRoutedEventArgs.Key == VirtualKey.Enter)
            {
                Interaction.ExecuteActions(this.AssociatedObject, this.Actions, keyRoutedEventArgs);
            }
        }
        

        private void OnOnKeyDownChanged(bool oldValue, bool newValue)
        {
            this.UnRegisterEvents(oldValue);
            this.RegisterEvents(newValue);
        }
    }
}