﻿using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using UwCore.Application;
using UwCore.Hamburger;
using UwCoreTest.Views.Test;

namespace UwCoreTest.ApplicationModes
{
    public class NormalApplicationMode : ApplicationMode
    {
        private readonly NavigatingHamburgerItem _testHamburgerItem;
        private readonly NavigatingHamburgerItem _test2HamburgerItem;

        public NormalApplicationMode()
        {
            this._testHamburgerItem = new NavigatingHamburgerItem("Test", Symbol.Contact, typeof(TestViewModel));
            this._testHamburgerItem.AddParameter<TestViewModel>(f => f.SomeId, 15);

            this._test2HamburgerItem = new NavigatingHamburgerItem("Test", Symbol.Contact, typeof(TestViewModel));
            this._test2HamburgerItem.AddParameter<TestViewModel>(f => f.SomeId, 13);
        }

        public override void Enter()
        {
            this.Application.Actions.Add(this._testHamburgerItem);
            this.Application.Actions.Add(this._test2HamburgerItem);

            this._testHamburgerItem.Execute();
        }

        public override void Leave()
        {
            this.Application.Actions.Remove(this._testHamburgerItem);
            this.Application.Actions.Remove(this._test2HamburgerItem);
        }
    }
}