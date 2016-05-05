using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;

namespace UwCore.Hamburger
{
    public sealed partial class HamburgerView : Page
    {
        public HamburgerView()
        {
            this.InitializeComponent();

            this.ContentFrame.ContentTransitions = new TransitionCollection
            {
                new NavigationThemeTransition
                {
                    DefaultNavigationTransitionInfo = new EntranceNavigationTransitionInfo()
                }
            };
        }

        public HamburgerViewModel ViewModel => this.DataContext as HamburgerViewModel;

        private void ListViewBase_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var clickedItem = (HamburgerItem)e.ClickedItem;
            clickedItem.Execute();

            if (this.WindowSize.CurrentState == this.Narrow)
            {
                this.Navigation.IsPaneOpen = false;
            }
        }

        private void OpenNavigationView(object sender, RoutedEventArgs e)
        {
            this.Navigation.IsPaneOpen = true;
        }

        private void NavigationPane_OnManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            if (this.WindowSize.CurrentState != this.Narrow)
                return;

            if (this.Navigation.IsPaneOpen)
                return;

            if (e.Cumulative.Translation.X > 20)
            {
                this.Navigation.IsPaneOpen = true;
            }
        }
    }
}
