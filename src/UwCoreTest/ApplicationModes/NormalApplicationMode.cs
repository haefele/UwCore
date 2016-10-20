﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Caliburn.Micro;
using UwCore.Application;
using UwCore.Events;
using UwCore.Hamburger;
using UwCoreTest.Views.HeaderDetails;
using UwCoreTest.Views.Test;

namespace UwCoreTest.ApplicationModes
{
    [AutoSubscribeEvents]
    public class NormalApplicationMode : ApplicationMode, ICustomStartupApplicationMode, IHandle<string>
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

        protected override async Task OnEnter()
        {
            await base.OnEnter();

            this._testHamburgerItem.Execute();
        }

        protected override async Task AddActions()
        {
            await base.AddActions();

            this.Application.Actions.Add(this._testHamburgerItem);
            this.Application.Actions.Add(this._test2HamburgerItem);
        }

        protected override async Task RemoveActions()
        {
            await base.RemoveActions();

            this.Application.Actions.Remove(this._testHamburgerItem);
            this.Application.Actions.Remove(this._test2HamburgerItem);
        }

        public void HandleCustomStartup(string tileId, string arguments)
        {
            var a = new MyStartupArguments
            {
                Id = 2,
                SomeString = "holla holla",
                Date = DateTimeOffset.Now
            };

            arguments = StartupArguments.AsString(a);
            var parsed = StartupArguments.Parse<MyStartupArguments>(arguments);
        }

        void IHandle<string>.Handle(string message)
        {

        }
    }

    public class MyStartupArguments
    {
        public int Id { get; set; }
        public string SomeString { get; set; }
        public DateTimeOffset Date { get; set; }
    }
}