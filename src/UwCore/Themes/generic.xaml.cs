using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Caliburn.Micro;
using UwCore.Controls;
using UwCore.Services.Navigation;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Themes
{
    public partial class generic : ResourceDictionary
    {
        public generic()
        {
            this.InitializeComponent();
        }
    }
}
