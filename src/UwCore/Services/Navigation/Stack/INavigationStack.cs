namespace UwCore.Services.Navigation.Stack
{
    public interface INavigationStack
    {
        void AddStep(INavigationStep step);
        bool RemoveStep(INavigationStep step);
    }
}