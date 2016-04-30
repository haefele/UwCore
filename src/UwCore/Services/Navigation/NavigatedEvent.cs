namespace UwCore.Services.Navigation
{
    public class NavigatedEvent
    {
        public object ViewModel { get; }

        public NavigatedEvent(object viewModel)
        {
            this.ViewModel = viewModel;
        }
    }
}