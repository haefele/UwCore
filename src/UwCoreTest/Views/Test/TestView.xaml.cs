using Windows.UI.Xaml.Controls;

namespace UwCoreTest.Views.Test
{
    public sealed partial class TestView : Page
    {
        public TestViewModel ViewModel => this.DataContext as TestViewModel;

        public TestView()
        {
            this.InitializeComponent();
        }
    }
}
