namespace UwCore.Application
{
    public interface ICustomStartupApplicationMode
    {
        void HandleCustomStartup(string tileId, string arguments);
    }
}