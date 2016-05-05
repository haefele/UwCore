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
        private readonly NavigatingHamburgerItem _test2HamburgerItem;

        public NormalApplicationMode(INavigationService navigationService)
        {
            this._navigationService = navigationService;

            this._testHamburgerItem = new NavigatingHamburgerItem("Test", Symbol.Contact, typeof(TestViewModel), new Dictionary<string, object>
            {
                [nameof(TestViewModel.SomeId)] = 15,
            });
            this._test2HamburgerItem = new NavigatingHamburgerItem("Test", Symbol.Contact, typeof(TestViewModel), new Dictionary<string, object>
            {
                [nameof(TestViewModel.SomeId)] = 13,
            });
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._testHamburgerItem);
            this.Application.Actions.Add(this._test2HamburgerItem);

            this._navigationService.For<TestViewModel>()
                .WithParam(f => f.SomeId, 15)
                .Navigate();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._testHamburgerItem);
            this.Application.Actions.Remove(this._test2HamburgerItem);
        }
    }
}