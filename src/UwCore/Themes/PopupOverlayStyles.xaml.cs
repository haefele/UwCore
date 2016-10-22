﻿using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using UwCore.Common;
using UwCore.Controls;

namespace UwCore.Themes
{
    public partial class PopupOverlayStyles : ResourceDictionary
    {
        public PopupOverlayStyles()
        {
            this.InitializeComponent();
        }


        private void PopupOverlayBackground_OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var popupOverlay = VisualTreeHelperEx.GetParent<PopupOverlay>((DependencyObject) sender);
            popupOverlay.Close();
        }

        private void ContentPresenter_OnLoaded(object sender, RoutedEventArgs e)
        {
            var contentPresenter = (ContentPresenter) sender;
            contentPresenter.RegisterPropertyChangedCallback(ContentPresenter.ContentProperty, this.ContentPresenter_OnContentChanged);

            this.ContentPresenter_OnContentChanged(contentPresenter, ContentPresenter.ContentProperty);
        }

        private void ContentPresenter_OnContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            var contentPresenter = (ContentPresenter) sender;
            var grid = (Grid)contentPresenter.Parent;
            var content = contentPresenter.Content as FrameworkElement;

            grid.MaxWidth = this.GetMax(content, f => f.Width, f => f.MaxWidth) 
                + grid.BorderThickness.Left 
                + grid.BorderThickness.Right;

            grid.MaxHeight = this.GetMax(content, f => f.Height, f => f.MaxHeight) 
                + grid.BorderThickness.Bottom 
                + grid.BorderThickness.Top;
        }

        private double GetMax(FrameworkElement element, Func<FrameworkElement, double> selector, Func<FrameworkElement, double> maxSelector)
        {
            if (element == null)
                return double.PositiveInfinity;

            if (double.IsInfinity(maxSelector(element)) == false && double.IsNaN(maxSelector(element)) == false)
                return maxSelector(element);

            if (double.IsInfinity(selector(element)) == false && double.IsNaN(selector(element)) == false)
                return selector(element);

            return double.PositiveInfinity;
        }
    }
}
