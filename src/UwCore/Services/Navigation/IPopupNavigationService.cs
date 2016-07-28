namespace UwCore.Services.Navigation
{
    public interface IPopupNavigationService
    {
        IAdvancedPopupNavigationService Advanced { get; }

        NavigateHelper<T> For<T>();
    }
}