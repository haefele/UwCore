using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro.ReactiveUI;
using ReactiveUI;
using UwCore.Common;

namespace UwCore
{
    public static class UwCoreCommand
    {
        public static UwCoreCommandBuilder<Unit> Create(Func<Task> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(null, async token =>
            {
                await execute();
                return Unit.Default;
            });
        }
        public static UwCoreCommandBuilder<T> Create<T>(Func<Task<T>> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(null, _ => execute());
        }
        public static UwCoreCommandBuilder<Unit> Create(Func<CancellationToken, Task> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(null, async token =>
            {
                await execute(token);
                return Unit.Default;
            });
        }
        public static UwCoreCommandBuilder<T> Create<T>(Func<CancellationToken, Task<T>> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(null, execute);
        }
        
        public static UwCoreCommandBuilder<Unit> Create(IObservable<bool> canExecute, Func<Task> execute)
        {
            Guard.NotNull(canExecute, nameof(canExecute));
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(canExecute, async token =>
            {
                await execute();
                return Unit.Default;
            });
        }
        public static UwCoreCommandBuilder<T> Create<T>(IObservable<bool> canExecute, Func<Task<T>> execute)
        {
            Guard.NotNull(canExecute, nameof(canExecute));
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(canExecute, _ => execute());
        }
        public static UwCoreCommandBuilder<Unit> Create(IObservable<bool> canExecute, Func<CancellationToken, Task> execute)
        {
            Guard.NotNull(canExecute, nameof(canExecute));
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(canExecute, async token =>
            {
                await execute(token);
                return Unit.Default;
            });
        }
        public static UwCoreCommandBuilder<T> Create<T>(IObservable<bool> canExecute, Func<CancellationToken, Task<T>> execute)
        {
            Guard.NotNull(canExecute, nameof(canExecute));
            Guard.NotNull(execute, nameof(execute));

            return CreateInternal(canExecute, execute);
        }

        private static UwCoreCommandBuilder<T> CreateInternal<T>(IObservable<bool> canExecute, Func<CancellationToken, Task<T>> execute)
        {
            return new UwCoreCommandBuilder<T>(canExecute, execute);
        }
    }

    public class UwCoreCommand<T> : ReactivePropertyChangedBase, ICommand
    {
        private readonly ReactiveCommand<T> _innerCommand;
        private bool _isExecuting;
        private bool _canExecute;

        internal UwCoreCommand(ReactiveCommand<T> innerCommand)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));

            this._innerCommand = innerCommand;
            this._innerCommand.IsExecuting.Subscribe(isExecuting => this.IsExecuting = isExecuting);
            this._innerCommand.CanExecuteObservable.Subscribe(canExecute => this.CanExecute = canExecute);
        }

        public bool IsExecuting
        {
            get { return this._isExecuting; }
            private set { this.RaiseAndSetIfChanged(ref this._isExecuting, value); }
        }

        public bool CanExecute
        {
            get { return this._canExecute; }
            private set { this.RaiseAndSetIfChanged(ref this._canExecute, value); }
        }

        public async Task<T> ExecuteAsync()
        {
            try
            {
                return await this._innerCommand.ExecuteAsyncTask();
            }
            catch
            {
                return default(T);
            }
        }

        public void ToProperty<TContainer>(TContainer source, Expression<Func<TContainer, T>> property, out ObservableAsPropertyHelper<T> result)
            where TContainer : ReactiveObject
        {
            Guard.NotNull(source, nameof(source));
            Guard.NotNull(property, nameof(property));

            this._innerCommand.ToProperty(source, property, out result);
            
            //Make sure the property is loaded
            source.WhenAnyValue(property)
                .Subscribe(_ => { });
        }

        #region Implementation of ICommand
        bool ICommand.CanExecute(object parameter)
        {
            return (this._innerCommand as ICommand).CanExecute(parameter);
        }

        async void ICommand.Execute(object parameter)
        {
            await this.ExecuteAsync();
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { ((ICommand) this._innerCommand).CanExecuteChanged += value; }
            remove { ((ICommand) this._innerCommand).CanExecuteChanged -= value; }
        }
        #endregion
    }
}