using System;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using UwCore.Extensions;
using UwCore.Services.Analytics;
using UwCore.Services.Dialog;

namespace UwCore.Services.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        private static readonly ILog Logger = LogManager.GetLog(typeof(ExceptionHandler));

        private readonly IDialogService _dialogService;
        private readonly IAnalyticsService _analyticsService;
        private readonly Type _commonExceptionType;
        private readonly string _errorMessage;
        private readonly string _errorTitle;

        public ExceptionHandler(IDialogService dialogService, IAnalyticsService analyticsService, Type commonExceptionType, string errorMessage, string errorTitle)
        {
            this._dialogService = dialogService;
            this._analyticsService = analyticsService;
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
                Logger.Error(exception);
                
                this._analyticsService.TrackException(exception);

                await this._dialogService.ShowAsync(this._errorMessage, this._errorTitle);
            }
        }
    }
}