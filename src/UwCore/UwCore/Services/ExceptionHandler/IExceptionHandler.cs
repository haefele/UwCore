using System;
using System.Threading.Tasks;

namespace UwCore.Services.ExceptionHandler
{
    public interface IExceptionHandler
    {
        Task HandleAsync(Exception exception);
    }
}