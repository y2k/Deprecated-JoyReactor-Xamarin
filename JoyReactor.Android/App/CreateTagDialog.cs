using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.App.Common;
using JoyReactor.Core.ViewModels;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;

namespace JoyReactor.Android.App
{
    public class CreateTagDialog : BaseDialogFragment
    {
        CreateTagViewModel viewmodel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            viewmodel = new CreateTagViewModel();
            MessengerInstance.Register<CreateTagViewModel.CloseMessage>(
                this, _ => DismissAllowingStateLoss());
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            var dialog = base.OnCreateDialog(savedInstanceState);
            dialog.SetTitle(Resource.String.create_tag);
            return dialog;
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.dialog_create_tag, null);
            Bindings.BeginScope(viewmodel);

            var name = view.FindViewById<EditText>(Resource.Id.name);
            name.SetBinding((s, v) => s.Error = Convert(v), () => viewmodel.NameError);
            name.SetBinding((s, v) => s.Text = v, () => viewmodel.Name)
                .SetTwoWay();
            view.FindViewById<ViewAnimator>(Resource.Id.animator)
                .SetBinding((s, v) => s.DisplayedChild = v ? 1 : 0, () => viewmodel.IsBusy);

            View.FindViewById(Resource.Id.cancel).Click += (sender, e) => Dismiss();
            View.FindViewById(Resource.Id.ok).SetCommand(viewmodel.CreateCommand);

            Bindings.EndScope();
            return view;
        }

        string Convert(bool isError)
        {
            return isError ? GetString(Resource.String.required_field) : null;
        }
    }
}