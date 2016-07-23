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
using Microsoft.HockeyApp;
using UwCore.Application.Events;
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
            this.ConfigureHockeyApp();
            this.ConfigureContainer();
            this.ConfigureCaliburnMicro();
            this.CustomConfiguration();
        }

        private void ConfigureHockeyApp()
        {
            var appId = this.GetHockeyAppId();

            if (string.IsNullOrWhiteSpace(appId) == false)
            {
                HockeyClient.Current.Configure(appId);
            }
        }

        private void ConfigureContainer()
        {
            this._container = new WinRTContainer();
            this._container.RegisterWinRTServices();

            //HockeyApp
            this._container.Instance(HockeyClient.Current);

            //Dialog
            this._container.Singleton<IDialogService, DialogService>();

            //Exception
            var dialogService = this._container.GetInstance<IDialogService>();
            var commonExceptionType = this.GetCommonExceptionType();
            var errorMessage = this.GetErrorMessage();
            var errorTitle = this.GetErrorTitle();
            this._container.Instance((IExceptionHandler) new ExceptionHandler(dialogService, this._container.GetInstance<IHockeyClient>(), commonExceptionType, errorMessage, errorTitle));

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
            MessageBinder.CustomConverters[typeof(DateTimeOffset)] = (value, context) =>
            {
                DateTimeOffset result;
                DateTimeOffset.TryParse(value.ToString(), out result);
                return result;
            };
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

            var view = new HamburgerView();
            this._container.Instance((INavigationService)new NavigationService(view.ContentFrame, this._container.GetInstance<IEventAggregator>(), this._container.GetInstance<IHockeyClient>()));
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));

            var viewModel = new HamburgerViewModel(IoC.Get<INavigationService>(), IoC.Get<IEventAggregator>(), IoC.Get<IHockeyClient>());
            this._container.Instance((IApplication)viewModel);

            this.CustomizeApplication(viewModel);

            viewModel.CurrentMode = this.GetCurrentMode();

            var customStartup = viewModel.CurrentMode as ICustomStartupApplicationMode;
            customStartup?.HandleCustomStartup(args.TileId, args.Arguments);

            ViewModelBinder.Bind(viewModel, view, null);
            ScreenExtensions.TryActivate(viewModel);

            Window.Current.Content = view;
            Window.Current.Activate();
        }

        protected override async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await IoC.Get<IApplicationStateService>().SaveStateAsync();
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationSuspending());

            deferral.Complete();
        }

        protected override void OnResuming(object sender, object e)
        {
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationResumed());
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

        public virtual void CustomizeApplication(IApplication application)
        {
            
        }
        
        public virtual void CustomConfiguration()
        {

        }

        public virtual string GetHockeyAppId()
        {
            return null;
        }

        public abstract ApplicationMode GetCurrentMode();
        public abstract string GetErrorTitle();
        public abstract string GetErrorMessage();
        public abstract Type GetCommonExceptionType();
        #endregion
    }
}