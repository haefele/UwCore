using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Services.Navigation;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Hamburger
{
    public class HamburgerViewModel : ReactiveScreen, IApplication, IHandle<NavigatedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;

        private HamburgerItem _selectedAction;
        private HamburgerItem _selectedSecondaryAction;
        private ApplicationMode _currentMode;

        private object _latestViewModel;

        public ReactiveObservableCollection<HamburgerItem> Actions { get; }

        public HamburgerItem SelectedAction
        {
            get { return this._selectedAction; }
            set { this.RaiseAndSetIfChanged(ref this._selectedAction, value); }
        }

        public ReactiveObservableCollection<HamburgerItem> SecondaryActions { get; }

        public HamburgerItem SelectedSecondaryAction
        {
            get { return this._selectedSecondaryAction; }
            set { this.RaiseAndSetIfChanged(ref this._selectedSecondaryAction, value); }
        }

        public ApplicationMode CurrentMode
        {
            get { return this._currentMode; }
            set
            {
                if (this._currentMode != null)
                {
                    this._currentMode.Leave();
                    this._eventAggregator.PublishOnCurrentThread(new ApplicationModeLeft(this._currentMode));
                }

                this._currentMode = value;
                this._currentMode.Application = this;

                this._currentMode.Enter();
                this._eventAggregator.PublishOnCurrentThread(new ApplicationModeEntered(this._currentMode));

                this._navigationService.ClearBackStack();
            }
        }

        public HamburgerViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;

            this.Actions = new ReactiveObservableCollection<HamburgerItem>();
            this.SecondaryActions = new ReactiveObservableCollection<HamburgerItem>();

            //Just make sure the selected action is always correct
            //Because it might happen, that we navigate to some view-model and then after that update the actions
            this.Actions.Changed.Subscribe(_ => this.UpdateSelectedAction());
            this.SecondaryActions.Changed.Subscribe(_ => this.UpdateSelectedAction());

            eventAggregator.Subscribe(this);
        }
        
        private void UpdateSelectedAction()
        {
            var selectedAction = this.Actions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel) && this.AreParametersEqual(f, this._latestViewModel));

            var selectedSecondaryAction = this.SecondaryActions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel) && this.AreParametersEqual(f, this._latestViewModel));

            if (selectedAction != null || selectedSecondaryAction != null)
            {
                this.SelectedAction = selectedAction;
                this.SelectedSecondaryAction = selectedSecondaryAction;
            }
        }

        private bool AreParametersEqual(NavigatingHamburgerItem hamburgerItem, object viewModel)
        {
            foreach (KeyValuePair<string, object> param in hamburgerItem.Parameters)
            {
                var property = viewModel.GetType().GetPropertyCaseInsensitive(param.Key);

                if (property == null)
                    continue;

                var expectedValue = MessageBinder.CoerceValue(property.PropertyType, param.Value, null);
                var actualValue = property.GetValue(viewModel);

                if (object.Equals(expectedValue, actualValue) == false)
                    return false;
            }

            return true;
        }
        
        void IHandle<NavigatedEvent>.Handle(NavigatedEvent message)
        {
            this._latestViewModel = message.ViewModel;
            this.UpdateSelectedAction();
        }
    }
}