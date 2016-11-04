namespace UwCore.Services.Navigation
{
    public interface INavigationStack
    {
        void AddStep(INavigationStackStep step);
        bool RemoveStep(INavigationStackStep step);
    }
}