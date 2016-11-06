﻿using System;
using System.Collections;
using System.Collections.Generic;
using Caliburn.Micro;

namespace UwCore
{
    public abstract class UwCoreConductorBase<T> : UwCoreScreen, IConductor, IParent<T> where T : class
    {
        /// <summary>
        /// Gets or sets the close strategy.
        /// </summary>
        /// <value>The close strategy.</value>
        public ICloseStrategy<T> CloseStrategy { get; set; }

        public UwCoreConductorBase()
        {
            this.CloseStrategy = new DefaultCloseStrategy<T>();
        }

        void IConductor.ActivateItem(object item)
        {
            this.ActivateItem((T)item);
        }

        void IConductor.DeactivateItem(object item, bool close)
        {
            this.DeactivateItem((T)item, close);
        }

        IEnumerable IParent.GetChildren()
        {
            return this.GetChildren();
        }

        /// <summary>
        /// Occurs when an activation request is processed.
        /// </summary>
        public virtual event EventHandler<ActivationProcessedEventArgs> ActivationProcessed = delegate { };

        /// <summary>
        /// Gets the children.
        /// </summary>
        /// <returns>The collection of children.</returns>
        public abstract IEnumerable<T> GetChildren();

        /// <summary>
        /// Activates the specified item.
        /// </summary>
        /// <param name="item">The item to activate.</param>
        public abstract void ActivateItem(T item);

        /// <summary>
        /// Deactivates the specified item.
        /// </summary>
        /// <param name="item">The item to close.</param>
        /// <param name="close">Indicates whether or not to close the item after deactivating it.</param>
        public abstract void DeactivateItem(T item, bool close);

        /// <summary>
        /// Called by a subclass when an activation needs processing.
        /// </summary>
        /// <param name="item">The item on which activation was attempted.</param>
        /// <param name="success">if set to <c>true</c> activation was successful.</param>
        protected virtual void OnActivationProcessed(T item, bool success)
        {
            if (item == null)
            {
                return;
            }

            this.ActivationProcessed?.Invoke(this, new ActivationProcessedEventArgs
            {
                Item = item,
                Success = success
            });
        }

        /// <summary>
        /// Ensures that an item is ready to be activated.
        /// </summary>
        /// <param name="newItem">The item that is about to be activated.</param>
        /// <returns>The item to be activated.</returns>
        protected virtual T EnsureItem(T newItem)
        {
            var node = newItem as IChild;

            if (node != null && node.Parent != this)
                node.Parent = this;

            return newItem;
        }
    }
}