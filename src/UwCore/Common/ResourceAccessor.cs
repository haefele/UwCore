using Windows.ApplicationModel.Resources;

namespace UwCore.Common
{
    public class ResourceAccessor
    {
        private readonly ResourceLoader _resourceLoader;

        public ResourceAccessor(ResourceLoader resourceLoader)
        {
            Guard.NotNull(resourceLoader, nameof(resourceLoader));

            this._resourceLoader = resourceLoader;
        }
        
        public string Get(string resource)
        {
            Guard.NotNullOrWhiteSpace(resource, nameof(resource));

            resource = resource.Replace(".", "/");

            return this._resourceLoader.GetString(resource);
        }
        
        public string GetFormatted(string resource, params object[] arguments)
        {
            Guard.NotNullOrWhiteSpace(resource, nameof(resource));

            return string.Format(this.Get(resource), arguments);
        }
    }
}