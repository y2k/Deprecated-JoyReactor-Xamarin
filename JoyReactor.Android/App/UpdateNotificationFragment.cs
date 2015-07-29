using Android.OS;
using Android.Views;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.App.Common;
using JoyReactor.Core.ViewModels;

namespace JoyReactor.Android.App
{
    public class UpdateNotificationFragment : BaseFragment
    {
        UpdateNotificationViewModel viewmodel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            viewmodel = new UpdateNotificationViewModel();
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_update_notification, container, false);
            Bindings.BeginScope(viewmodel);

            view.SetBinding((s, v) => s.SetVisibility(v), () => viewmodel.UpdateAvailable);
            view.FindViewById(Resource.Id.open).SetCommand(viewmodel.OpenCommand);

            Bindings.EndScope();
            return view;
        }
    }
}