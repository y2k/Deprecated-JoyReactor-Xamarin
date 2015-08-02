using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Support.V4.Widget;
using Android.Views;
using System.ComponentModel;
using Android.Widget;

namespace JoyReactor.Android.App.Common
{
    public static class ViewExtensions
    {
        public static Task WaitPreDrawAsync(this ViewTreeObserver instance)
        {
            var locker = new TaskCompletionSource<object>();
            var listener = new PreDrawListener { parent = instance, PreDraw = () => locker.TrySetResult(null) };
            instance.AddOnPreDrawListener(listener);
            return locker.Task;
        }

        public static Task EndAsync(this ViewPropertyAnimator instance)
        {
            var locker = new TaskCompletionSource<object>();
            instance.WithEndAction(new Java.Lang.Runnable(() => locker.TrySetResult(null)));
            return locker.Task;
        }

        public static ViewStates ToViewStates(this bool instance)
        {
            return instance ? ViewStates.Visible : ViewStates.Gone;
        }

        public static void SetVisibility(this View instance, bool visibility)
        {
            instance.Visibility = visibility ? ViewStates.Visible : ViewStates.Gone;
        }

        public static void SetCommand(this SwipeRefreshLayout instance, ICommand command)
        {
            instance.Refresh += (sender, e) => command.Execute(null);
        }

        public static void SetCommand(this View instance, ICommand command, object param = null)
        {
            instance.SetClick((sender, e) => command.Execute(param));
        }

        static readonly List<ClickRecord> records = new List<ClickRecord>();

        public static void SetClick(this View view, EventHandler listener)
        {
            for (int i = records.Count - 1; i >= 0; i--)
            {
                var s = records[i];
                View v;
                if (s.view.TryGetTarget(out v))
                {
                    if (v == view)
                    {
                        records.RemoveAt(i);
                        view.Click -= s.handler;
                        break;
                    }
                }
                else
                    records.RemoveAt(i);
            }

            view.Click += listener;
            records.Add(new ClickRecord { view = new WeakReference<View>(view), handler = listener });
        }


        internal static BindableEditText ToBindable(this EditText instance)
        {
            return new BindableEditText(instance);
        }

        struct ClickRecord
        {
            public WeakReference<View> view;
            public EventHandler handler;
        }

        class PreDrawListener : Java.Lang.Object, ViewTreeObserver.IOnPreDrawListener
        {
            internal Action PreDraw { get; set; }

            internal ViewTreeObserver parent { get; set; }

            public bool OnPreDraw()
            {
                PreDraw();
                parent.RemoveOnPreDrawListener(this);
                return false;
            }
        }

        internal class BindableEditText : INotifyPropertyChanged
        {
            public string Text
            {
                get { return view.Text; }
                set
                {
                    if (view.Text != (value ?? ""))
                        view.Text = value;
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            readonly EditText view;

            public BindableEditText(EditText view)
            {
                this.view = view;
                view.TextChanged += (sender, e) => InvokeChanged();
            }

            void InvokeChanged()
            {
                PropertyChanged.Invoke(view, new PropertyChangedEventArgs(nameof(Text)));
            }
        }
    }
}