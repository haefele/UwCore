using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Caliburn.Micro;
using ReactiveUI;

namespace UwCore
{
    public abstract class UwCorePropertyChangedBase : ReactiveObject, INotifyPropertyChangedEx
    {
        public void NotifyOfPropertyChange([CallerMemberName]string propertyName = null)
        {
            this.RaisePropertyChanged(propertyName);
        }

        public void Refresh()
        {
            this.NotifyOfPropertyChange(string.Empty);
        }

        public bool IsNotifying
        {
            get { return this.AreChangeNotificationsEnabled(); }
            set { throw new NotSupportedException("Use the SupressNotifications method instead."); }
        }
    }
}