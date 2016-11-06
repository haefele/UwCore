using System;
using System.Threading.Tasks;
using Windows.Storage.Pickers.Provider;

namespace UwCore.Services.ApplicationState
{
    public interface IApplicationStateService
    {
        T Get<T>(string key, ApplicationState state);
        void Set<T>(string key, T value, ApplicationState state);

        Task SaveStateAsync();
        Task RestoreStateAsync();

        IApplicationStateService GetStateServiceFor(Type type);
    }

    public enum ApplicationState
    {
        Local,
        Roaming,
        Vault,
        Temp,
    }
}