using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Uwp.UI;

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
