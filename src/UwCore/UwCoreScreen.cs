using System;
using Caliburn.Micro;
using UwCore.Services.ApplicationState;
using IReactiveObjectExtensions = ReactiveUI.IReactiveObjectExtensions;

namespace UwCore
{
    public abstract class UwCoreScreen : UwCoreViewAware, IScreen, IChild
    {
        private static readonly ILog Log = LogManager.GetLog(typeof(UwCoreScreen));

        private bool _isActive;
        private bool _isInitialized;
        private object _parent;
        private string _displayName;

        /// <summary>
        /// Creates an instance of the screen.
        /// </summary>
        public UwCoreScreen()
        {
            this._displayName = this.GetType().FullName;
        }

        /// <summary>
        /// Gets or Sets the Parent <see cref = "IConductor" />
        /// </summary>
        public virtual object Parent
        {
            get { return this._parent; }
            set { IReactiveObjectExtensions.RaiseAndSetIfChanged(this, ref this._parent, value); }
        }

        /// <summary>
        /// Gets or Sets the Display Name
        /// </summary>
        public virtual string DisplayName
        {
            get { return this._displayName; }
            set { IReactiveObjectExtensions.RaiseAndSetIfChanged(this, ref this._displayName, value); }
        }

        /// <summary>
        /// Indicates whether or not this instance is currently active.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsActive
        {
            get { return this._isActive; }
            private set { IReactiveObjectExtensions.RaiseAndSetIfChanged(this, ref this._isActive, value); }
        }

        /// <summary>
        /// Indicates whether or not this instance is currently initialized.
        /// Virtualized in order to help with document oriented view models.
        /// </summary>
        public virtual bool IsInitialized
        {
            get { return this._isInitialized; }
            private set { IReactiveObjectExtensions.RaiseAndSetIfChanged(this, ref this._isInitialized, value); }
        }

        /// <summary>
        /// Raised after activation occurs.
        /// </summary>
        public virtual event EventHandler<ActivationEventArgs> Activated = delegate { };

        /// <summary>
        /// Raised before deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> AttemptingDeactivation = delegate { };

        /// <summary>
        /// Raised after deactivation.
        /// </summary>
        public virtual event EventHandler<DeactivationEventArgs> Deactivated = delegate { };

        void IActivate.Activate()
        {
            if (this.IsActive)
            {
                return;
            }

            var initialized = false;

            if (!this.IsInitialized)
            {
                this.IsInitialized = initialized = true;

                this.RestoreState(IoC.Get<IApplicationStateService>().GetStateServiceFor(this.GetType()));
                this.OnInitialize();
            }

            this.IsActive = true;
            Log.Info("Activating {0}.", this);
            this.OnActivate();

            this.Activated?.Invoke(this, new ActivationEventArgs
            {
                WasInitialized = initialized
            });
        }

        /// <summary>
        /// Called before <see cref="OnInitialize"/>. Use it to restore your previously saved state.
        /// </summary>
        /// <param name="applicationStateService">The application state service specifically for this class.</param>
        protected virtual void RestoreState(IApplicationStateService applicationStateService)
        {
            
        }

        /// <summary>
        /// Called after <see cref="OnDeactivate"/>. Use it to save your state.
        /// </summary>
        /// <param name="applicationStateService">The application state service specifically for this class.</param>
        protected virtual void SaveState(IApplicationStateService applicationStateService)
        {
            
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected virtual void OnInitialize()
        {

        }

        /// <summary>
        /// Called when activating.
        /// </summary>
        protected virtual void OnActivate()
        {
            
        }

        void IDeactivate.Deactivate(bool close)
        {
            if (this.IsActive || (this.IsInitialized && close))
            {
                this.AttemptingDeactivation?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                this.IsActive = false;
                Log.Info("Deactivating {0}.", this);

                this.OnDeactivate(close);
                this.SaveState(IoC.Get<IApplicationStateService>().GetStateServiceFor(this.GetType()));

                this.Deactivated?.Invoke(this, new DeactivationEventArgs
                {
                    WasClosed = close
                });

                if (close)
                {
                    this.Views.Clear();
                    Log.Info("Closed {0}.", this);
                }
            }
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name = "close">Inidicates whether this instance will be closed.</param>
        protected virtual void OnDeactivate(bool close)
        {
            
        }

        /// <summary>
        /// Called to check whether or not this instance can close.
        /// </summary>
        /// <param name = "callback">The implementor calls this action with the result of the close check.</param>
        public virtual void CanClose(Action<bool> callback)
        {
            callback(true);
        }

        /// <summary>
        /// Tries to close this instance by asking its Parent to initiate shutdown or by asking its corresponding view to close.
        /// Also provides an opportunity to pass a dialog result to it's corresponding view.
        /// </summary>
        /// <param name="dialogResult">The dialog result.</param>
        public virtual void TryClose(bool? dialogResult = null)
        {
            PlatformProvider.Current.GetViewCloseAction(this, this.Views.Values, dialogResult).OnUIThread();
        }
    }
}