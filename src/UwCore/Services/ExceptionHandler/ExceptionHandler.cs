using System;
using System.Reflection;
using System.Threading.Tasks;
using Caliburn.Micro;
using Microsoft.HockeyApp;
using UwCore.Extensions;
using UwCore.Services.Dialog;

namespace UwCore.Services.ExceptionHandler
{
    public class ExceptionHandler : IExceptionHandler
    {
        private static readonly Caliburn.Micro.ILog Logger = LogManager.GetLog(typeof(ExceptionHandler));

        private readonly IDialogService _dialogService;
        private readonly IHockeyClient _hockeyClient;
        private readonly Type _commonExceptionType;
        private readonly string _errorMessage;
        private readonly string _errorTitle;
        private readonly bool _isHockeyClientConfigured;

        public ExceptionHandler(IDialogService dialogService, IHockeyClient hockeyClient, Type commonExceptionType, string errorMessage, string errorTitle, bool isHockeyClientConfigured)
        {
            this._dialogService = dialogService;
            this._hockeyClient = hockeyClient;
            this._commonExceptionType = commonExceptionType;
            this._errorMessage = errorMessage;
            this._errorTitle = errorTitle;
            this._isHockeyClientConfigured = isHockeyClientConfigured;
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
                
                if (this._isHockeyClientConfigured)
                    this._hockeyClient.TrackException(exception);

                await this._dialogService.ShowAsync(this._errorMessage, this._errorTitle);
            }
        }
    }
}