using System.Threading.Tasks;

namespace UwCore.Application
{
    public abstract class ApplicationMode
    {
        public IApplication Application { get; set; }

        internal async Task Enter()
        {
            await this.AddActions();
            await this.OnEnter();
        }

        internal async Task Leave()
        {
            await this.RemoveActions();
            await this.OnLeave();
        }

        protected virtual Task OnEnter()
        {
            return Task.CompletedTask;
        }

        protected virtual Task OnLeave()
        {
            return Task.CompletedTask;
        }

        protected virtual Task AddActions()
        {
            return Task.CompletedTask;
        }

        protected virtual Task RemoveActions()
        {
            return Task.CompletedTask;
        }

        protected async Task RefreshActions()
        {
            await this.RemoveActions();
            await this.AddActions();
        }
    }
}