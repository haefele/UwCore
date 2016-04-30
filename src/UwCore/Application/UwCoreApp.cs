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
using UwCore.Services.Loading;
using UwCore.Services.Navigation;
using INavigationService = Caliburn.Micro.INavigationService;

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
            

            this.RegisterComponents(this._container);
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

            await this.RestoreStateAsync();

            var view = new ShellView();
            this._container.Instance((INavigationService)new NavigationService(view.ContentFrame, this._container.GetInstance<IEventAggregator>()));
            this._container.Instance((ILoadingService)new LoadingService(view.LoadingOverlay));

            var viewModel = IoC.Get<ShellViewModel>();
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

            await this.SaveStateAsync();
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

        public virtual void RegisterComponents(WinRTContainer container)
        {
            
        }

        public virtual Task RestoreStateAsync()
        {
            return Task.CompletedTask;
        }

        public virtual Task SaveStateAsync()
        {
            return Task.CompletedTask;
        }

        public abstract ApplicationMode GetCurrentMode();

        #endregion
    }
}