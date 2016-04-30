using System;
using System.Reflection;
using System.Threading.Tasks;
using UwCore.Extensions;
using UwCore.Logging;
using UwCore.Services.Dialog;

namespace UwCore.Services.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        private static readonly Logger Logger = LoggerFactory.GetLogger<ExceptionHandler>();

        private readonly IDialogService _dialogService;
        private readonly Type _commonExceptionType;
        private readonly string _errorMessage;
        private readonly string _errorTitle;

        public ExceptionHandler(IDialogService dialogService, Type commonExceptionType, string errorMessage, string errorTitle)
        {
            this._dialogService = dialogService;
            this._commonExceptionType = commonExceptionType;
            this._errorMessage = errorMessage;
            this._errorTitle = errorTitle;
        }

        public async Task HandleAsync(Exception exception)
        {
            if (this._commonExceptionType.IsInstanceOfType(exception))
            {
                await this._dialogService.ShowAsync(exception.GetFullMessage(), this._errorTitle);
            }
            else
            {
                Logger.Error(exception, "Handled exception occurred.");

                await this._dialogService.ShowAsync(this._errorMessage, this._errorTitle);
            }
        }
    }
}