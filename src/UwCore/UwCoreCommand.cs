using System;
using System.Linq.Expressions;
using System.Reactive;
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

            return Create(token => execute());
        }

        public static UwCoreCommandBuilder<Unit> Create(Func<CancellationToken, Task> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return Create(async token =>
            {
                await execute(token);
                return Unit.Default;
            });
        }

        public static UwCoreCommandBuilder<T> Create<T>(Func<Task<T>> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return Create(_ => execute());
        }

        public static UwCoreCommandBuilder<T> Create<T>(Func<CancellationToken, Task<T>> execute)
        {
            Guard.NotNull(execute, nameof(execute));

            return new UwCoreCommandBuilder<T>(execute);
        }
    }

    public class UwCoreCommand<T> : ICommand
    {
        private readonly ReactiveCommand<T> _innerCommand;

        internal UwCoreCommand(ReactiveCommand<T> innerCommand)
        {
            Guard.NotNull(innerCommand, nameof(innerCommand));

            this._innerCommand = innerCommand;
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
            return ((ICommand) this._innerCommand).CanExecute(parameter);
        }

        void ICommand.Execute(object parameter)
        {
            ((ICommand) this._innerCommand).Execute(parameter);
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { ((ICommand) this._innerCommand).CanExecuteChanged += value; }
            remove { ((ICommand) this._innerCommand).CanExecuteChanged -= value; }
        }
        #endregion
    }
}