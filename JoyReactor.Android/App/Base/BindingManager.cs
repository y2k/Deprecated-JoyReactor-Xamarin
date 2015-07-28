using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Android.Widget;
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

        [Obsolete]
        public Binding<TS, TT> Add<TS, TT>(object source, Expression<Func<TS>> sourceExpression, object target, Expression<Func<TT>> targetExpression, BindingMode mode = BindingMode.Default)
        {
            var binding = source.SetBinding(sourceExpression, target, targetExpression, mode);
            bindings.Add(binding);
            return binding;
        }

        [Obsolete]
        public Binding Add(object source, Expression<Func<string>> sourceExpression, EditText target)
        {
            var prop = (PropertyInfo)((MemberExpression)sourceExpression.Body).Member;
            var binding = Add(source, sourceExpression)
                .WhenSourceChanges(() =>
                {
                    var text = (string)prop.GetValue(source);
                    if (target.Text != text)
                        target.Text = text;
                });
            target.TextChanged += (sender, e) => prop.SetValue(source, target.Text);
            return binding;
        }

        public void Destroy()
        {
            bindings.Clear();
            delegates.Clear();
        }
    }
}