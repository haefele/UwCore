using System.Linq;
using System.Reflection;
using Caliburn.Micro;
using UwCore.Application;
using UwCore.Extensions;
using UwCore.Services.Navigation;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Hamburger
{
    public class HamburgerViewModel : Screen, IApplication, IHandle<NavigatedEvent>
    {
        private readonly INavigationService _navigationService;

        private HamburgerItem _selectedAction;
        private HamburgerItem _selectedSecondaryAction;
        private ApplicationMode _currentMode;

        private object _latestViewModel;

        public BindableCollection<HamburgerItem> Actions { get; }

        public HamburgerItem SelectedAction
        {
            get { return this._selectedAction; }
            set { this.SetProperty(ref this._selectedAction, value); }
        }

        public BindableCollection<HamburgerItem> SecondaryActions { get; }

        public HamburgerItem SelectedSecondaryAction
        {
            get { return this._selectedSecondaryAction; }
            set { this.SetProperty(ref this._selectedSecondaryAction, value); }
        }

        public ApplicationMode CurrentMode
        {
            get { return this._currentMode; }
            set
            {
                this._currentMode?.Leave();

                this._currentMode = value;
                this._currentMode.Application = this;

                this._currentMode?.Enter();

                this._navigationService.ClearBackStack();
            }
        }

        public HamburgerViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
        {
            this._navigationService = navigationService;

            this.Actions = new BindableCollection<HamburgerItem>();
            this.SecondaryActions = new BindableCollection<HamburgerItem>();

            //Just make sure the selected action is always correct
            //Because it might happen, that we navigate to some view-model and then after that update the actions
            this.Actions.CollectionChanged += (s, e) => this.UpdateSelectedAction();
            this.SecondaryActions.CollectionChanged += (s, e) => this.UpdateSelectedAction();

            eventAggregator.Subscribe(this);
        }
        
        private void UpdateSelectedAction()
        {
            var selectedAction = this.Actions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel));

            var selectedSecondaryAction = this.SecondaryActions
                .OfType<NavigatingHamburgerItem>()
                .FirstOrDefault(f => f.ViewModelType.IsInstanceOfType(this._latestViewModel));

            if (selectedAction != null || selectedSecondaryAction != null)
            {
                this.SelectedAction = selectedAction;
                this.SelectedSecondaryAction = selectedSecondaryAction;
            }
        }
        
        void IHandle<NavigatedEvent>.Handle(NavigatedEvent message)
        {
            this._latestViewModel = message.ViewModel;
            this.UpdateSelectedAction();
        }

        public void ExecuteAction(HamburgerItem hamburgerItem)
        {
            var navigating = hamburgerItem as NavigatingHamburgerItem;
            if (navigating != null)
            {
                this._navigationService.Navigate(navigating.ViewModelType);
            }

            var clickable = hamburgerItem as ClickableHamburgerItem;
            if (clickable != null)
            {
                clickable.Action();
            }
        }
    }
}