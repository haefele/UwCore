using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Core;

namespace UwCore.Services.Navigation.Stack
{
    public class NavigationStack : INavigationStack
    {
        private readonly IList<INavigationStep> _steps;

        public NavigationStack()
        {
            this._steps = new List<INavigationStep>();

            var navigationManager = SystemNavigationManager.GetForCurrentView();
            navigationManager.BackRequested += (s, e) =>
            {
                foreach (var step in this._steps.Reverse())
                {
                    if (step.CanGoBack())
                    {
                        step.GoBack();

                        this.StepOnChanged(this, EventArgs.Empty);

                        e.Handled = true;
                        return;
                    }
                }
            };
        }

        public void AddStep(INavigationStep step)
        {
            this._steps.Add(step);

            step.Changed += this.StepOnChanged;
        }

        public bool RemoveStep(INavigationStep step)
        {
            var removed = this._steps.Remove(step);

            if (removed)
            {
                step.Changed -= this.StepOnChanged;
            }

            return removed;
        }
        
        private void StepOnChanged(object sender, EventArgs eventArgs)
        {
            var systemNavigationManager = SystemNavigationManager.GetForCurrentView();

            systemNavigationManager.AppViewBackButtonVisibility = this._steps.Any(f => f.CanGoBack()) 
                ? AppViewBackButtonVisibility.Visible 
                : AppViewBackButtonVisibility.Collapsed;
        }
    }
}