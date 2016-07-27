using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwCore.Controls
{
    [TemplatePart(Name = NavigationButtonPartName, Type = typeof(Button))]
    public class Header : ContentControl
    {
        #region Template Parts
        private const string NavigationButtonPartName = "PART_NavigationButton";
        #endregion

        #region Fields
        private Button _navigationButton;
        #endregion

        #region Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
            nameof(Title), typeof(string), typeof(Header), new PropertyMetadata(default(string)));

        public string Title
        {
            get { return (string)this.GetValue(TitleProperty); }
            set { this.SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty NavigationButtonVisibilityProperty = DependencyProperty.Register(
            nameof(NavigationButtonVisibility), typeof(Visibility), typeof(Header), new PropertyMetadata(default(Visibility)));

        public Visibility NavigationButtonVisibility
        {
            get { return (Visibility)this.GetValue(NavigationButtonVisibilityProperty); }
            set { this.SetValue(NavigationButtonVisibilityProperty, value); }
        }
        #endregion

        #region Events
        public event EventHandler<RoutedEventArgs> NavigationButtonClick;
        #endregion

        #region Constructors
        public Header()
        {
            this.DefaultStyleKey = typeof(Header);
        }
        #endregion

        #region Private Methods
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.ExtractControlsFromTemplate();

            this._navigationButton.Click += (s, e) =>
            {
                this.NavigationButtonClick?.Invoke(this, new RoutedEventArgs());
            };
        }

        private void ExtractControlsFromTemplate()
        {
            this._navigationButton = (Button)this.GetTemplateChild(NavigationButtonPartName);
        }
        #endregion
    }
}
