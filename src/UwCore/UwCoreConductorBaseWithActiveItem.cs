using Caliburn.Micro;

namespace UwCore
{
    public abstract class UwCoreConductorBaseWithActiveItem<T> : UwCoreConductorBase<T>, IConductActiveItem where T : class
    {
        private T _activeItem;

        /// <summary>
        /// The currently active item.
        /// </summary>
        public T ActiveItem
        {
            get { return this._activeItem; }
            set { this.ActivateItem(value); }
        }

        /// <summary>
        /// The currently active item.
        /// </summary>
        /// <value></value>
        object IHaveActiveItem.ActiveItem
        {
            get { return this.ActiveItem; }
            set { this.ActiveItem = (T)value; }
        }

        /// <summary>
        /// Changes the active item.
        /// </summary>
        /// <param name="newItem">The new item to activate.</param>
        /// <param name="closePrevious">Indicates whether or not to close the previous active item.</param>
        protected virtual void ChangeActiveItem(T newItem, bool closePrevious)
        {
            ScreenExtensions.TryDeactivate(this._activeItem, closePrevious);

            newItem = this.EnsureItem(newItem);

            if (this.IsActive)
                ScreenExtensions.TryActivate(newItem);

            this._activeItem = newItem;
            this.NotifyOfPropertyChange(nameof(this.ActiveItem));
            this.OnActivationProcessed(this._activeItem, true);
        }
    }
}