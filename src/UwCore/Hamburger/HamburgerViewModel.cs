﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.UI.Xaml;
using Caliburn.Micro;
using ReactiveUI;
using UwCore.Application;
using UwCore.Application.Events;
using UwCore.Common;
using UwCore.Extensions;
using UwCore.Services.Analytics;
using UwCore.Services.Navigation;
using UwCore.Services.Navigation.Stack;
using UwCore.Services.UpdateNotes;
using DynamicData;
using DynamicData.Binding;

namespace UwCore.Hamburger
{
    public class HamburgerViewModel : UwCoreScreen, IShell
    {
        private readonly NavigationService _navigationService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IAnalyticsService _analyticsService;
        private readonly IUpdateNotesService _updateNotesService;

        private HamburgerItem _selectedAction;
        private HamburgerItem _selectedSecondaryAction;
        private ElementTheme _theme;
        private ShellMode _currentMode;
        private object _headerDetailsViewModel;

        private object _latestViewModel;

        public ObservableCollectionExtended<HamburgerItem> Actions { get; }

        public HamburgerItem SelectedAction
        {
            get { return this._selectedAction; }
            set { this.RaiseAndSetIfChanged(ref this._selectedAction, value); }
        }

        public ObservableCollectionExtended<HamburgerItem> SecondaryActions { get; }

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

        public ShellMode CurrentMode
        {
            get { return this._currentMode; }
            set
            {
                if (this._currentMode == value)
                    return;

                if (this._currentMode != null)
                {
                    this._currentMode.Leave().Wait();
                    this._eventAggregator.PublishOnCurrentThread(new ShellModeLeft(this._currentMode));
                }

                this._currentMode = value;
                this._currentMode.Shell = this;

                this._currentMode.Enter().Wait();
                this._eventAggregator.PublishOnCurrentThread(new ShellModeEntered(this._currentMode));

                this._analyticsService.TrackEvent("ShellModeChanged", new Dictionary<string, string> { ["ShellMode"] = this._currentMode.GetType().Name });
                
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

        public HamburgerViewModel(NavigationService navigationService, IEventAggregator eventAggregator, IAnalyticsService analyticsService, IUpdateNotesService updateNotesService)
        {
            Guard.NotNull(navigationService, nameof(navigationService));
            Guard.NotNull(eventAggregator, nameof(eventAggregator));
            Guard.NotNull(analyticsService, nameof(analyticsService));

            this._navigationService = navigationService;
            this._eventAggregator = eventAggregator;
            this._analyticsService = analyticsService;
            this._updateNotesService = updateNotesService;

            this.Theme = ElementTheme.Default;

            ((INavigationStep)this._navigationService).Changed += this.OnChanged;

            this.Actions = new ObservableCollectionExtended<HamburgerItem>();
            this.SecondaryActions = new ObservableCollectionExtended<HamburgerItem>();

            //Just make sure the selected action is always correct
            //Because it might happen, that we navigate to some view-model and then after that update the actions
            this.Actions.ToObservableChangeSet().Subscribe(_ => this.UpdateSelectedAction());
            this.SecondaryActions.ToObservableChangeSet().Subscribe(_ => this.UpdateSelectedAction());
        }

        private void OnChanged(object sender, NavigationStepChangedEventArgs eventArgs)
        {
            if (eventArgs.ViewModel != null)
                this._latestViewModel = eventArgs.ViewModel;

            this.UpdateSelectedAction();
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
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel) && ParametersHelper.AreParameterInjected(this._latestViewModel, f.Parameters));

            var selectedSecondaryAction = this.SecondaryActions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel) && ParametersHelper.AreParameterInjected(this._latestViewModel, f.Parameters));

            if (selectedAction != null || selectedSecondaryAction != null)
            {
                this.SelectedAction = selectedAction;
                this.SelectedSecondaryAction = selectedSecondaryAction;
            }
        }
    }
}