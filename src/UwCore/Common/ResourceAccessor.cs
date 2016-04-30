using Windows.ApplicationModel.Resources;
using JetBrains.Annotations;

namespace UwCore.Common
{
    public class ResourceAccessor
    {
        private readonly ResourceLoader _resourceLoader;

        public ResourceAccessor([NotNull]ResourceLoader resourceLoader)
        {
            Guard.NotNull(resourceLoader, nameof(resourceLoader));

            this._resourceLoader = resourceLoader;
        }

        [NotNull]
        public string Get([NotNull]string resource)
        {
            Guard.NotNullOrWhiteSpace(resource, nameof(resource));

            resource = resource.Replace(".", "/");

            return this._resourceLoader.GetString(resource);
        }

        [NotNull]
        public string GetFormatted([NotNull]string resource, params object[] arguments)
        {
            Guard.NotNullOrWhiteSpace(resource, nameof(resource));

            return string.Format(this.Get(resource), arguments);
        }
    }
}