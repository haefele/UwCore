using System;
using Caliburn.Micro;
using ReactiveUI;
using UwCore.Services.ExceptionHandler;
using UwCore.Services.Loading;

namespace UwCore.Extensions
{
    public static class ReactiveCommandExtensions
    {
        public static void AttachLoadingService(this IReactiveCommand self, string message)
        {
            IDisposable currentMessage = null;
            self.IsExecuting.Subscribe(f =>
            {
                if (f)
                {
                    currentMessage = IoC.Get<ILoadingService>().Show(message);
                }
                else
                {
                    currentMessage?.Dispose();
                }
            });
        }

        public static void AttachExceptionHandler(this IReactiveCommand self)
        {
            var exceptionHandler = IoC.Get<IExceptionHandler>();
            self.ThrownExceptions.Subscribe(async e => await exceptionHandler.HandleAsync(e));
        }
    }
}