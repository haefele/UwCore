using System;

namespace UwCore.Services.Loading
{
    public interface ILoadingService
    {
        IDisposable Show(string message);
    }
}