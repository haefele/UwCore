using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Caliburn.Micro;
using Caliburn.Micro.ReactiveUI;
using Microsoft.HockeyApp;
using ReactiveUI;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.Navigation;
using UwCore.Services.UpdateNotes;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Hamburger
{
    public class HamburgerViewModel : ReactiveScreen, IApplication, IHandle<NavigatedEvent>
    {
        private readonly INavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IHockeyClient _hockeyClient;
        private readonly IUpdateNotesService _updateNotesService;

        private HamburgerItem _selectedAction;
        private HamburgerItem _selectedSecondaryAction;
        private ElementTheme _theme;
        private ApplicationMode _currentMode;
        private object _headerDetailsViewModel;

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

        public ElementTheme Theme
        {
            get { return this._theme; }
            set { this.RaiseAndSetIfChanged(ref this._theme, value); }
        }

        public ApplicationMode CurrentMode
        {
            get { return this._currentMode; }
            set
            {
                if (this._currentMode == value)
                    return;

                if (this._currentMode != null)
                {
                    this._currentMode.Leave().Wait();
                    this._eventAggregator.PublishOnCurrentThread(new ApplicationModeLeft(this._currentMode));
                }

                this._currentMode = value;
                this._currentMode.Application = this;

                this._currentMode.Enter().Wait();
                this._eventAggregator.PublishOnCurrentThread(new ApplicationModeEntered(this._currentMode));

                this._hockeyClient.TrackEvent("ApplicationModeChanged", new Dictionary<string, string> { ["ApplicationMode"] = this._currentMode.GetType().Name });
                
                this._navigationService.ClearBackStack();

                this.RaisePropertyChanged();
            }
        }

        public object HeaderDetailsViewModel
        {
            get { return this._headerDetailsViewModel; }
            set
            {
                if (this._headerDetailsViewModel == value)
                    return;

                if (this._headerDetailsViewModel != null)
                {
                    ScreenExtensions.TryDeactivate(this._headerDetailsViewModel, true);
                }

                this._headerDetailsViewModel = value;

                ScreenExtensions.TryActivate(this._headerDetailsViewModel);
                
                this.RaisePropertyChanged();
            }
        }

        public HamburgerViewModel(INavigationService navigationService, IEventAggregator eventAggregator, IHockeyClient hockeyClient, IUpdateNotesService updateNotesService)
        {
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(hockeyClient, nameof(hockeyClient));

            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
            this._hockeyClient = hockeyClient;
            this._updateNotesService = updateNotesService;

            this.Theme = ElementTheme.Default;

            this.Actions = new ReactiveObservableCollection<HamburgerItem>();
            this.SecondaryActions = new ReactiveObservableCollection<HamburgerItem>();

            //Just make sure the selected action is always correct
            //Because it might happen, that we navigate to some view-model and then after that update the actions
            this.Actions.Changed.Subscribe(_ => this.UpdateSelectedAction());
            this.SecondaryActions.Changed.Subscribe(_ => this.UpdateSelectedAction());

            eventAggregator.Subscribe(this);
        }

        protected override void OnActivate()
        {
            base.OnActivate();

            var updateNotesViewModel = UwCoreApp.Current.GetUpdateNotesViewModelType();
            if (updateNotesViewModel != null && this._updateNotesService.HasSeenUpdateNotes() == false)
            {
                this._navigationService.Popup.Advanced.Navigate(updateNotesViewModel);

                this._updateNotesService.MarkUpdateNotesAsSeen();
            }
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