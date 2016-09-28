using System;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using ReactiveUI;
using UwCore.Common;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace UwCore
{
    public class UwCoreCommandBuilder<T>
    {
        private readonly Func<CancellationToken, Task<T>> _execute;
        private Func<string> _loadingServiceMessage;
        private bool _handleExceptions;
        private string _eventName;
        private IObservable<bool> _canExecute;
        
        public UwCoreCommandBuilder(Func<CancellationToken, Task<T>> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            this._execute = execute;
        }

        public UwCoreCommandBuilder<T> CanExecute(IObservable<bool> canExecute)
        {
            Guard.NotNull(canExecute, nameof(canExecute));

            this._canExecute = canExecute;

            return this;
        }

        public UwCoreCommandBuilder<T> ShowLoadingService(string message)
        {
            Guard.NotNullOrWhiteSpace(message, nameof(message));

            return this.ShowLoadingService(() => message);
        }

        public UwCoreCommandBuilder<T> ShowLoadingService(Func<string> message)
        {
            Guard.NotNull(message, nameof(message));

            this._loadingServiceMessage = message;

            return this;
        }

        public UwCoreCommandBuilder<T> HandleExceptions()
        {
            this._handleExceptions = true;

            return this;
        }

        public UwCoreCommandBuilder<T> TrackEvent(string eventName)
        {
            Guard.NotNullOrWhiteSpace(eventName, nameof(eventName));

            this._eventName = eventName;

            return this;
        }

        public static implicit operator UwCoreCommand<T>(UwCoreCommandBuilder<T> builder)
        {
            ReactiveCommand<T> innerCommand = ReactiveCommand.CreateAsyncTask(
                builder._canExecute ?? Observable.Return(true),
                async (_, token) => await builder._execute(token));

            if (builder._loadingServiceMessage != null)
                AttachLoadingService(innerCommand, builder._loadingServiceMessage);

            if (builder._handleExceptions)
                AttachExceptionHandler(innerCommand);

            if (builder._eventName != null)
                TrackEvent(innerCommand, builder._eventName);

            return new UwCoreCommand<T>(innerCommand);
        }

        private static void AttachLoadingService(IReactiveCommand innerCommand, Func<string> message)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));
            Guard.NotNull(message, nameof(message));

            IDisposable currentMessage = null;
            innerCommand.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    currentMessage = IoC.Get<ILoadingService>().Show(message());
                }
                else
                {
                    currentMessage?.Dispose();
                }
            });
        }

        private static void AttachExceptionHandler(IReactiveCommand innerCommand)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));

            var exceptionHandler = IoC.Get<IExceptionHandler>();
            innerCommand.ThrownExceptions.Subscribe(async e => await exceptionHandler.HandleAsync(e));
        }

        private static void TrackEvent(IReactiveCommand innerCommand, string eventName)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));
            Guard.NotNullOrWhiteSpace(eventName, nameof(eventName));

            innerCommand.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    IoC.Get<IHockeyClient>().TrackEvent(eventName);
                }
            });
        }
    }
}