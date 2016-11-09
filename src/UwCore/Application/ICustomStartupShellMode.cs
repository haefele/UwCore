namespace UwCore.Application
{
    public interface ICustomStartupShellMode
    {
        void HandleCustomStartup(string tileId, string arguments);
    }
}