namespace UwCore.Services.Navigation
{
    internal class NavigatedEvent
    {
        public object ViewModel { get; }

        public NavigatedEvent(object viewModel)
        {
            this.ViewModel = viewModel;
        }
    }
}