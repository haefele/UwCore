using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using UwCore.Application;
using UwCore.Hamburger;
using UwCore.Services.Navigation;
using UwCoreTest.Views.Test;

namespace UwCoreTest.ApplicationModes
{
    public class NormalApplicationMode : ApplicationMode
    {
        private readonly INavigationService _navigationService;

        private readonly HamburgerItem _testHamburgerItem;

        public NormalApplicationMode(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this._testHamburgerItem = new NavigatingHamburgerItem("Test", Symbol.Contact, typeof(TestViewModel));
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._testHamburgerItem);

            this._navigationService.Navigate(typeof(TestViewModel), new Dictionary<string, object>
            {
                [nameof(TestViewModel.SomeId)] = 15,
            });
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._testHamburgerItem);
        }
    }
}