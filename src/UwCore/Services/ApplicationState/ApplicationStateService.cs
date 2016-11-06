using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Storage;
using Newtonsoft.Json;
using UwCore.Common;

namespace UwCore.Services.ApplicationState
{
    public class ApplicationStateService : IApplicationStateService
    {
        #region Fields
        private ConcurrentDictionary<string, string> _localState;
        private ConcurrentDictionary<string, string> _roamingState;
        private ConcurrentDictionary<string, string> _vaultState;
        private ConcurrentDictionary<string, string> _tempState;

        private readonly JsonSerializerSettings _jsonSerializerSettings;
        #endregion

        #region Constructors
        public ApplicationStateService()
        {
            this._localState = new ConcurrentDictionary<string, string>();
            this._roamingState = new ConcurrentDictionary<string, string>();
            this._vaultState = new ConcurrentDictionary<string, string>();
            this._tempState = new ConcurrentDictionary<string, string>();

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

        public bool HasValueFor(string key, ApplicationState state)
        {
            var stateValues = this.GetStateValues(state);
            return stateValues.ContainsKey(key);
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

        public IApplicationStateService GetStateServiceFor(Type type)
        {
            Guard.NotNull(type, nameof(type));

            return new ForTypeApplicationStateService(this, type, "~");
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
                case ApplicationState.Temp:
                    return this._tempState;
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

        #region Internal
        private class ForTypeApplicationStateService : IApplicationStateService
        {
            private readonly IApplicationStateService _parent;
            private readonly Type _forType;
            private readonly string _additionalPrefix;

            public ForTypeApplicationStateService(IApplicationStateService parent, Type forType, string additionalPrefix = null)
            {
                Guard.NotNull(parent, nameof(parent));
                Guard.NotNull(forType, nameof(forType));

                this._parent = parent;
                this._forType = forType;
                this._additionalPrefix = additionalPrefix;
            }

            public T Get<T>(string key, ApplicationState state)
            {
                string fullKey = this.GetFullKey(key);
                return this._parent.Get<T>(fullKey, state);
            }

            public void Set<T>(string key, T value, ApplicationState state)
            {
                string fullKey = this.GetFullKey(key);
                this._parent.Set<T>(fullKey, value, state);
            }

            public bool HasValueFor(string key, ApplicationState state)
            {
                string fullKey = this.GetFullKey(key);
                return this._parent.HasValueFor(fullKey, state);
            }

            public Task SaveStateAsync()
            {
                return Task.CompletedTask;
            }

            public Task RestoreStateAsync()
            {
                return Task.CompletedTask;
            }

            public IApplicationStateService GetStateServiceFor(Type type)
            {
                return new ForTypeApplicationStateService(this, type);
            }

            private string GetFullKey(string key)
            {
                return $"{this._additionalPrefix}[{this._forType.Name}].{key}";
            }
        }
        #endregion
    }
}