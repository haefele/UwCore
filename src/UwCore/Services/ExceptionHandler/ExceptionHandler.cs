using System;
using System.Reflection;
using System.Threading.Tasks;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Dialog;

namespace UwCore.Services.ExceptionHandler
{
    public abstract class ExceptionHandler : IExceptionHandler
    {
        private static readonly Logger Logger = LoggerFactory.GetLogger<ExceptionHandler>();

        private readonly IDialogService _dialogService;

        public ExceptionHandler(IDialogService dialogService)
        {
            this._dialogService = dialogService;
        }
        
        public async Task HandleAsync(Exception exception)
        {
            if (this.GetCommonExceptionType().IsInstanceOfType(exception))
            {
                await this._dialogService.ShowAsync(exception.GetFullMessage(), this.GetErrorTitle());
            }
            else
            {
                Logger.Error(exception, "Handled exception occurred.");

                await this._dialogService.ShowAsync(this.GetErrorMessage(), this.GetErrorTitle());
            }
        }

        public abstract string GetErrorTitle();
        public abstract string GetErrorMessage();
        public abstract Type GetCommonExceptionType();
    }
}