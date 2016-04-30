using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Caliburn.Micro;
using UwCore.Extensions;
using UwCore.Hamburger;
using UwCore.Logging;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;
using UwCore.Services.Navigation;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Application
{
    public abstract class UwCoreApp : CaliburnApplication
    {
        #region Logger
        private static readonly Logger Logger = LoggerFactory.GetLogger<UwCoreApp>();
        #endregion

        #region Fields
        private WinRTContainer _container;
        #endregion
        
        #region Configure
        protected override void Configure()
        {
            this.ConfigureContainer();
            this.ConfigureCaliburnMicro();
        }

        private void ConfigureContainer()
        {
            this._container = new WinRTContainer();
            this._container.RegisterWinRTServices();

            //Dialog
            this._container.Singleton<IDialogService, DialogService>();

            //Exception
            var dialogService = this._container.GetInstance<IDialogService>();
            var commonExceptionType = this.GetCommonExceptionType();
            var errorMessage = this.GetErrorMessage();
            var errorTitle = this.GetErrorTitle();
            this._container.Instance((IExceptionHandler) new ExceptionHandler(dialogService, commonExceptionType, errorMessage, errorTitle));

            //ApplicationState
            this._container.Singleton<IApplicationStateService, ApplicationStateService>();
            
            //ViewModels
            var viewModelTypes = this.GetViewModelTypes();
            foreach (var viewModelType in viewModelTypes)
            {
                this._container.RegisterPerRequest(viewModelType, null, viewModelType);
            }

            //Other
            this.ConfigureContainer(this._container);
        }

        private void ConfigureCaliburnMicro()
        {
            ViewModelBinder.ApplyConventionsByDefault = false;
            LogManager.GetLog = type => new CaliburnMicroLoggingAdapter(LoggerFactory.GetLogger(type));
        }
        #endregion

        #region IoC
        protected override object GetInstance(Type service, string key)
        {
            return this._container.GetInstance(service, key);
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return this._container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
        }
        #endregion

        #region Lifecycle
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;

            this.Initialize();

            await IoC.Get<IApplicationStateService>().RestoreStateAsync();

            var view = new ShellView();
            this._container.Instance((INavigationService)new NavigationService(view.ContentFrame, this._container.GetInstance<IEventAggregator>()));
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));

            var viewModel = new ShellViewModel(IoC.Get<INavigationService>(), IoC.Get<IEventAggregator>());
            this._container.Instance((IApplication)viewModel);

            viewModel.CurrentMode = this.GetCurrentMode();

            ViewModelBinder.Bind(viewModel, view, null);
            ScreenExtensions.TryActivate(viewModel);

            Window.Current.Content = view;
            Window.Current.Activate();
        }

        protected override async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await IoC.Get<IApplicationStateService>().SaveStateAsync();
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationSuspendingEvent());

            deferral.Complete();
        }

        protected override void OnResuming(object sender, object e)
        {
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationResumedEvent());
        }

        protected override void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Error(e.Exception, "Unhandled exception occured.");
        }
        #endregion

        #region To Override 
        public virtual IEnumerable<Type> GetViewModelTypes()
        {
            yield break;
        }

        public virtual void ConfigureContainer(WinRTContainer container)
        {
            
        }
        
        public abstract ApplicationMode GetCurrentMode();
        public abstract string GetErrorTitle();
        public abstract string GetErrorMessage();
        public abstract Type GetCommonExceptionType();
        #endregion
    }
}