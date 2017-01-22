using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
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

    public class UwCoreCommand<T> : UwCorePropertyChangedBase, ICommand
    {
        private readonly ReactiveCommand<object, T> _innerCommand;
        private readonly ObservableAsPropertyHelper<bool> _isExecutingHelper;
        private readonly ObservableAsPropertyHelper<bool> _canExecuteHelper;

        internal UwCoreCommand(ReactiveCommand<object, T> innerCommand)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));

            this._innerCommand = innerCommand;
            this._innerCommand.IsExecuting.ToProperty(this, f => f.IsExecuting, out this._isExecutingHelper);
            this._innerCommand.CanExecute.ToProperty(this, f => f.CanExecute, out this._canExecuteHelper);
        }

        public bool IsExecuting
        {
            get { return this._isExecutingHelper.Value ; }
        }

        public bool CanExecute
        {
            get { return this._canExecuteHelper.Value; }
        }

        public async Task<T> ExecuteAsync()
        {
            try
            {
                return await this._innerCommand.Execute();
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