using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using GalaSoft.MvvmLight.Helpers;

namespace JoyReactor.Android.App.Base
{
    public class BindingManager
    {
        public static BindingManager Scope { get; private set; }

        public object DataContext { get; private set; }

        public void BeginScope(object dataContext)
        {
            DataContext = dataContext;
            Scope = this;
        }

        public void EndScope()
        {
            DataContext = null;
            Scope = null;
        }

        readonly List<Binding> bindings = new List<Binding>();
        readonly List<object> delegates = new List<object>();

        public Binding<T, T> Add<T>(object source, Expression<Func<T>> sourceExpression, BindingMode mode = BindingMode.Default)
        {
            var binding = source.SetBinding(sourceExpression, mode);
            bindings.Add(binding);
            return binding;
        }

        public void Add(Action onChangedDelegate)
        {
            delegates.Add(onChangedDelegate);
        }

        public void Destroy()
        {
            bindings.Clear();
            delegates.Clear();
        }
    }
}