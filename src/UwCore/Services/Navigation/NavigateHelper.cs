using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Caliburn.Micro;

namespace UwCore.Services.Navigation
{
    public class NavigateHelper<TViewModel>
    {
        private readonly Action<Type, Dictionary<string, object>> _navigate;
        private readonly Dictionary<string, object> _parameters;

        public NavigateHelper(Action<Type, Dictionary<string, object>> navigate)
        {
            this._navigate = navigate;
            this._parameters = new Dictionary<string, object>();
        }

        public NavigateHelper<TViewModel> WithParam<TValue>(Expression<Func<TViewModel, TValue>> property, TValue value)
        {
            if (value is ValueType || !ReferenceEquals(null, value))
            {
                this._parameters[property.GetMemberInfo().Name] = value;
            }

            return this;
        }

        public void Navigate()
        {
            this._navigate(typeof(TViewModel), this._parameters);
        }
    }
}