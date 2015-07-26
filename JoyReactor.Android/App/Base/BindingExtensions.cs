using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using GalaSoft.MvvmLight.Helpers;

namespace JoyReactor.Android.App.Base
{
    public static class BindingExtensions
    {
        public static InnerBinding<T, TS, TS> SetBinding<T, TS>(this T target, Action<T, TS> whenSourceChanged, Expression<Func<TS>> sourceExpression)
        {
            var source = BindingManager.Scope.DataContext;
            var sourceProperty = (PropertyInfo)((MemberExpression)sourceExpression.Body).Member;

            Action action = () => whenSourceChanged(target, (TS)sourceProperty.GetValue(source));
            BindingManager.Scope.Add(action);
            var binding = BindingManager.Scope
                .Add(source, sourceExpression, BindingMode.OneWay)
                .WhenSourceChanges(action);

            return new InnerBinding<T, TS, TS>
            {
                binding = binding,
                target = target,
                source = source,
                sourceProperty = sourceProperty,
            };
        }

        public class InnerBinding<T, TS, TT>
        {
            internal object target;
            internal object source;
            internal PropertyInfo sourceProperty;
            internal Binding<TS, TS> binding;

            public void SetTwoWay(Expression<Func<T, TT>> targetExpression)
            {
                var targetProperty = (PropertyInfo)((MemberExpression)targetExpression.Body).Member;

                if (target is INotifyPropertyChanged)
                {
                    var observable = (INotifyPropertyChanged)target;
                    observable.PropertyChanged += (sender, e) =>
                    {
                        if (e.PropertyName == targetProperty.Name)
                            SetSource(targetProperty.GetValue(target));
                    };
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            void SetSource(object value)
            {
                sourceProperty.SetValue(source, value);
            }
        }
    }
}