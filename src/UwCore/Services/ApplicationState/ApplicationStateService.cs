using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Newtonsoft.Json;

namespace UwCore.Services.ApplicationState
{
    public class ApplicationStateService : IApplicationStateService
    {
        #region Fields
        private ConcurrentDictionary<string, string> _localState;
        private ConcurrentDictionary<string, string> _roamingState;
        private ConcurrentDictionary<string, string> _vaultState;

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        #endregion

        #region Constructors
        public ApplicationStateService()
        {
            this._localState = new ConcurrentDictionary<string, string>();
            this._roamingState = new ConcurrentDictionary<string, string>();
            this._vaultState = new ConcurrentDictionary<string, string>();

            this._jsonSerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
        }
        #endregion

        #region Implementation of IApplicationStateService
        public T Get<T>(string key, ApplicationState state)
        {
            var stateValues = this.GetStateValues(state);

            string found;
            if (stateValues.TryGetValue(key, out found))
            {
                return JsonConvert.DeserializeObject<T>(found, this._jsonSerializerSettings);
            }

            return default(T);
        }

        public void Set<T>(string key, T value, ApplicationState state)
        {
            var stateValues = this.GetStateValues(state);

            string valueAsString = JsonConvert.SerializeObject(value, this._jsonSerializerSettings);
            stateValues.AddOrUpdate(key, valueAsString, (_, __) => valueAsString);
        }

        public async Task SaveStateAsync()
        {
            await this.SaveLocalStateAsync();
            await this.SaveRoamingStateAsync();
            await this.SaveVaultStateAsync();
        }

        public async Task RestoreStateAsync()
        {
            await this.RestoreLocalStateAsync();
            await this.RestoreRoamingStateAsync();
            await this.RestoreVaultStateAsync();
        }
        #endregion

        #region Private Methods
        private ConcurrentDictionary<string, string> GetStateValues(ApplicationState state)
        {
            switch (state)
            {
                case ApplicationState.Local:
                    return this._localState;
                case ApplicationState.Roaming:
                    return this._roamingState;
                case ApplicationState.Vault:
                    return this._vaultState;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        private async Task SaveLocalStateAsync()
        {
            var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("ApplicationState.bin", CreationCollisionOption.ReplaceExisting);
            await this.SaveFileStateAsync(file, this._localState);
        }

        private async Task RestoreLocalStateAsync()
        {
            try
            {
                var file = await ApplicationData.Current.LocalFolder.CreateFileAsync("ApplicationState.bin", CreationCollisionOption.OpenIfExists);
                this._localState = await this.RestoreFileStateAsync(file) ?? new ConcurrentDictionary<string, string>();
            }
            catch
            {
                this._localState = new ConcurrentDictionary<string, string>();
            }
        }

        private async Task SaveRoamingStateAsync()
        {
            var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("ApplicationState.bin", CreationCollisionOption.ReplaceExisting);
            await this.SaveFileStateAsync(file, this._roamingState);
        }

        private async Task RestoreRoamingStateAsync()
        {
            try
            {
                var file = await ApplicationData.Current.RoamingFolder.CreateFileAsync("ApplicationState.bin", CreationCollisionOption.OpenIfExists);
                this._roamingState = await this.RestoreFileStateAsync(file) ?? new ConcurrentDictionary<string, string>();
            }
            catch
            {
                this._roamingState = new ConcurrentDictionary<string, string>();
            }
        }

        private Task SaveVaultStateAsync()
        {
            string json = JsonConvert.SerializeObject(this._vaultState, this._jsonSerializerSettings);

            var vault = new PasswordVault();
            var credentials = new PasswordCredential("UwCore", "ApplicationState", json);
            vault.Add(credentials);

            return Task.CompletedTask;
        }

        private Task RestoreVaultStateAsync()
        {

            try
            {
                var vault = new PasswordVault();
                var credentials = vault.Retrieve("UwCore", "ApplicationState");
                credentials.RetrievePassword();

                this._vaultState = JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(credentials.Password, this._jsonSerializerSettings) ?? new ConcurrentDictionary<string, string>();
                return Task.CompletedTask;
            }
            catch
            {
                this._vaultState = new ConcurrentDictionary<string, string>();
                return Task.CompletedTask;
            }
        }
        #endregion

        #region Basic
        private async Task SaveFileStateAsync(StorageFile file, ConcurrentDictionary<string, string> state)
        {
            using (var stream = await file.OpenStreamForWriteAsync())
            using (var writer = new StreamWriter(stream, Encoding.UTF8))
            {
                var json = JsonConvert.SerializeObject(state, this._jsonSerializerSettings);
                await writer.WriteAsync(json);
            }
        }
        
        private async Task<ConcurrentDictionary<string, string>> RestoreFileStateAsync(StorageFile file)
        {
            using (var stream = await file.OpenStreamForReadAsync())
            using (var reader = new StreamReader(stream, Encoding.UTF8))
            {
                var json = await reader.ReadToEndAsync();
                return JsonConvert.DeserializeObject<ConcurrentDictionary<string, string>>(json, this._jsonSerializerSettings);
            }
        }
        #endregion
    }
}