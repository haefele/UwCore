using System;
using System.Collections.Generic;
using System.Linq;
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
using UwCore.Events;
using UwCore.Extensions;
using UwCore.Hamburger;
using UwCore.Logging;
using UwCore.Services.ApplicationState;
using UwCore.Services.Dialog;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;
using UwCore.Services.Navigation;
using UwCore.Services.UpdateNotes;
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

        #region Properties
        public new static UwCoreApp Current => Windows.UI.Xaml.Application.Current as UwCoreApp;
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

            if (string.IsNullOrWhiteSpace(appId) == false && this.IsHockeyAppEnabled())
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
            
            //UpdateNotes
            this._container.Singleton<IUpdateNotesService, UpdateNotesService>();

            //ViewModels
            var viewModelTypes = this.GetViewModelTypes();
            foreach (var viewModelType in viewModelTypes)
            {
                this._container.RegisterPerRequest(viewModelType, null, viewModelType);
            }

            //ApplicationModes
            var applicationModeTypes = this.GetApplicationModeTypes();
            foreach (var applicationModeType in applicationModeTypes)
            {
                this._container.RegisterPerRequest(applicationModeType, null, applicationModeType);
            }

            //Services
            var serviceTypes = this.GetServiceTypes().ToList();
            for (int i = 0; i < serviceTypes.Count; i += 2)
            {
                if (serviceTypes.Count <= i + 1)
                    throw new InvalidOperationException($"There is an error in your override of '{nameof(this.GetServiceTypes)}'. Make sure you always return the service-type first, and then the implementation-type.");

                var serviceType = serviceTypes[i];
                var implementationType = serviceTypes[i + 1];

                this._container.RegisterSingleton(serviceType, null, implementationType);
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
            var instance = this._container.GetInstance(service, key);
            this.TryAutoSubscribeToEventAggregator(instance);
            return instance;
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            var instances = this._container.GetAllInstances(service).ToList();
            this.TryAutoSubscribeToEventAggregator(instances);
            return instances;
        }

        protected override void BuildUp(object instance)
        {
            this._container.BuildUp(instance);
            this.TryAutoSubscribeToEventAggregator(instance);
        }

        private void TryAutoSubscribeToEventAggregator(params object[] instances)
        {
            foreach (object instance in instances)
            {
                if (instance == null)
                    continue;

                TypeInfo typeInfo = instance.GetType().GetTypeInfo();

                if (typeInfo.IsDefined(typeof(AutoSubscribeEventsAttribute)))
                    this._container.GetInstance<IEventAggregator>().Subscribe(instance);

                if (instance is IScreen && typeInfo.IsDefined(typeof(AutoSubscribeEventsForScreenAttribute)))
                    this._container.GetInstance<IEventAggregator>().SubscribeScreen((IScreen)instance);
            }
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
            this._container.Instance((INavigationService)new NavigationService(view.ContentFrame, view.PopupOverlay, this._container.GetInstance<IEventAggregator>(), this._container.GetInstance<IHockeyClient>()));
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));

            var viewModel = new HamburgerViewModel(IoC.Get<INavigationService>(), IoC.Get<IEventAggregator>(), IoC.Get<IHockeyClient>(), IoC.Get<IUpdateNotesService>());
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

        public virtual IEnumerable<Type> GetApplicationModeTypes()
        {
            yield break;
        }

        public virtual IEnumerable<Type> GetServiceTypes()
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

        public virtual bool IsHockeyAppEnabled()
        {
            var currentVersion = Package.Current.Id.Version.ToVersion();
            return currentVersion != Version.Parse("1.0.0.0");
        }

        public virtual Type GetUpdateNotesViewModelType()
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