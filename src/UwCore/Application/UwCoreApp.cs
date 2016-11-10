using System;
using System.Collections;
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
using Autofac;
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
using UwCore.Services.Navigation.Stack;
using UwCore.Services.UpdateNotes;
using INavigationService = UwCore.Services.Navigation.INavigationService;

namespace UwCore.Application
{
    public abstract class UwCoreApp : Windows.UI.Xaml.Application
    {
        #region Fields
        private bool _isInitialized;
        private IContainer _container;
        #endregion

        #region Properties
        public new static UwCoreApp Current => Windows.UI.Xaml.Application.Current as UwCoreApp;
        #endregion
        
        #region Lifecycle
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            base.OnLaunched(args);

            if (args.PreviousExecutionState == ApplicationExecutionState.Running ||
                args.PreviousExecutionState == ApplicationExecutionState.Suspended)
                return;
            
            await this.Initialize();

            var builder = new ContainerBuilder();

            var view = new HamburgerView();
            var popupNavigationService = new PopupNavigationService(view.PopupOverlay);
            var navigationService = new NavigationService(view.ContentFrame, this._container.Resolve<IEventAggregator>(), popupNavigationService);

            var stack = new NavigationStack();
            stack.AddStep(navigationService);
            stack.AddStep(popupNavigationService);

            builder.RegisterInstance(stack)
                .As<INavigationStack>()
                .SingleInstance();
            builder.RegisterInstance(navigationService)
                .As<INavigationService>()
                .SingleInstance();
            builder.RegisterInstance(new LoadingService(view.LoadingOverlay))
                .As<ILoadingService>()
                .SingleInstance();

            var viewModel = new HamburgerViewModel(navigationService, IoC.Get<IEventAggregator>(), IoC.Get<IHockeyClient>(), IoC.Get<IUpdateNotesService>());
            builder.RegisterInstance(viewModel)
                .As<IShell>()
                .SingleInstance();

            builder.Update(this._container);

            this.CustomizeShell(viewModel);

            viewModel.CurrentMode = this.GetCurrentMode();

            var customStartup = viewModel.CurrentMode as ICustomStartupShellMode;
            customStartup?.HandleCustomStartup(args.TileId, args.Arguments);

            ViewModelBinder.Bind(viewModel, view, null);
            ScreenExtensions.TryActivate(viewModel);

            Window.Current.Content = view;
            Window.Current.Activate();
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            base.OnWindowCreated(args);

            // Because dispatchers are tied to windows Execute will fail in 
            // scenarios when the app has multiple windows open (though contract 
            // activation, this keeps Excute up to date with the currently activated window
            args.Window.Activated += (s, e) => PlatformProvider.Current = new UwCorePlatformProvider();
        }

        private async void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();

            await IoC.Get<IApplicationStateService>().SaveStateAsync();
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationSuspending());

            deferral.Complete();
        }

        private void OnResuming(object sender, object e)
        {
            IoC.Get<IEventAggregator>().PublishOnCurrentThread(new ApplicationResumed());
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            LogManager.GetLog(typeof(UwCoreApp)).Error(e.Exception);
        }
        #endregion

        #region Private Methods
        private async Task Initialize()
        {
            //Initialize only once
            if (this._isInitialized)
                return;

            this._isInitialized = true;

            //Caliburn Micro Setup
            PlatformProvider.Current = new UwCorePlatformProvider();
            EventAggregator.HandlerResultProcessing = (target, result) => 
            {
                var task = result as Task;
                if (task != null)
                {
                    result = new IResult[] { task.AsResult() };
                }

                var coroutine = result as IEnumerable<IResult>;
                if (coroutine != null)
                {
                    var viewAware = target as IViewAware;
                    var view = viewAware?.GetView();

                    var context = new CoroutineExecutionContext
                    {
                        Target = target,
                        View = view
                    };

                    Coroutine.BeginExecute(coroutine.GetEnumerator(), context);
                }
            };
            AssemblySource.Assemblies.AddRange(this.SelectAssemblies());

            //Attach to application events
            this.Resuming += this.OnResuming;
            this.Suspending += this.OnSuspending;
            this.UnhandledException += this.OnUnhandledException;

            //Configure
            this.ConfigureHockeyApp();
            this.ConfigureContainer();
            this.ConfigureCaliburnMicro();
            this.Configure();

            //Setup IoC
            IoC.GetInstance = (service, key) => this._container.IsRegistered(service) 
                ? this._container.Resolve(service) 
                : Activator.CreateInstance(service);
            IoC.GetAllInstances = service => ((IEnumerable) this._container.Resolve(typeof(IEnumerable<>).MakeGenericType(service))).OfType<object>();
            IoC.BuildUp = instance => this._container.InjectUnsetProperties(instance);
            
            //Restore state
            await IoC.Get<IApplicationStateService>().RestoreStateAsync();
        }
        #endregion
        
        #region Configure
        private void ConfigureHockeyApp()
        {
            var appId = this.GetHockeyAppId();

            if (string.IsNullOrWhiteSpace(appId) == false && this.IsHockeyAppEnabled())
            {
                HockeyClient.Current
                    .Configure(appId)
                    .SetExceptionDescriptionLoader(f => string.Join(Environment.NewLine, InMemoryLogMessages.GetLogs()));
            }
        }

        private void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterCallback(x =>
            {
                x.Registered += (s, e) =>
                {
                    e.ComponentRegistration.Activated += (ss, ee) =>
                    {
                        if (ee.Instance == null)
                            return;

                        TypeInfo typeInfo = ee.Instance.GetType().GetTypeInfo();

                        if (typeInfo.IsDefined(typeof(AutoSubscribeEventsAttribute)))
                            this._container.Resolve<IEventAggregator>().Subscribe(ee.Instance);

                        if (ee.Instance is IScreen && typeInfo.IsDefined(typeof(AutoSubscribeEventsForScreenAttribute)))
                            this._container.Resolve<IEventAggregator>().SubscribeScreen((IScreen)ee.Instance);
                    };
                };
            });

            //Container itself
            builder.Register(cc => this._container)
                .As<IContainer>()
                .SingleInstance();

            //Event aggregator
            builder.RegisterType<EventAggregator>()
                .As<IEventAggregator>()
                .SingleInstance();

            //HockeyApp
            builder.RegisterInstance(HockeyClient.Current)
                .As<IHockeyClient>()
                .SingleInstance();

            //Dialog
            builder.RegisterType<DialogService>()
                .As<IDialogService>()
                .SingleInstance();

            //Exception
            builder.Register(cc => new ExceptionHandler(
                                       cc.Resolve<IDialogService>(), 
                                       cc.Resolve<IHockeyClient>(), 
                                       this.GetCommonExceptionType(), 
                                       this.GetErrorMessage(), 
                                       this.GetErrorTitle()))
                .As<IExceptionHandler>()
                .SingleInstance();

            //ApplicationState
            builder.RegisterType<ApplicationStateService>()
                .As<IApplicationStateService>()
                .SingleInstance();

            //UpdateNotes
            builder.RegisterType<UpdateNotesService>()
                .As<IUpdateNotesService>()
                .SingleInstance();

            //ViewModels
            var viewModelTypes = this.GetViewModelTypes();
            foreach (var viewModelType in viewModelTypes)
            {
                builder.RegisterType(viewModelType);
            }

            //ApplicationModes
            var applicationModeTypes = this.GetShellModeTypes();
            foreach (var applicationModeType in applicationModeTypes)
            {
                builder.RegisterType(applicationModeType);
            }

            //Services
            var serviceTypes = this.GetServiceTypes().ToList();
            for (int i = 0; i < serviceTypes.Count; i += 2)
            {
                if (serviceTypes.Count <= i + 1)
                    throw new InvalidOperationException($"There is an error in your override of '{nameof(this.GetServiceTypes)}'. Make sure you always return the service-type first, and then the implementation-type.");

                var serviceType = serviceTypes[i];
                var implementationType = serviceTypes[i + 1];

                builder.RegisterType(implementationType)
                    .As(serviceType)
                    .SingleInstance();
            }

            //Other
            this.ConfigureContainer(builder);

            this._container = builder.Build();
        }

        private void ConfigureCaliburnMicro()
        {
            HockeyLogManager.GetLog = type => new LogAdapter(type);
            LogManager.GetLog = type => new LogAdapter(type);
        }
        #endregion

        #region To Override 
        public virtual IEnumerable<Type> GetViewModelTypes()
        {
            yield break;
        }

        public virtual IEnumerable<Type> GetShellModeTypes()
        {
            yield break;
        }

        public virtual IEnumerable<Type> GetServiceTypes()
        {
            yield break;
        }

        public virtual void ConfigureContainer(ContainerBuilder builder)
        {
            
        }

        public virtual void CustomizeShell(IShell shell)
        {
            
        }
        
        public virtual void Configure()
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

        public virtual IEnumerable<Assembly> SelectAssemblies()
        {
            yield return this.GetType().GetTypeInfo().Assembly;
        }

        public abstract ShellMode GetCurrentMode();
        public abstract string GetErrorTitle();
        public abstract string GetErrorMessage();
        public abstract Type GetCommonExceptionType();
        #endregion
    }
}