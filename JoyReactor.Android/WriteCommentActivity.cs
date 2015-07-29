using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.App.Common;
using JoyReactor.Android.Widget;
using JoyReactor.Core.ViewModels;

namespace JoyReactor.Android
{
    [Activity(
        Theme = "@style/AppTheme.WriteComment",
        WindowSoftInputMode = SoftInput.AdjustResize
    )]
    public class WriteCommentActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_write_comment);

            var viewmodel = Scope.New<WriteCommentViewModel>();
            Bindings.BeginScope(viewmodel);

            var sendButton = FindViewById(Resource.Id.send);
            sendButton.SetBinding((s, v) => s.Enabled = v, () => viewmodel.CanSend);
            FindViewById(Resource.Id.progress)
                .SetBinding((s, v) => s.SetVisibility(v), () => viewmodel.IsBusy);
            FindViewById<WebImageView>(Resource.Id.userImage)
                .SetBinding((s, v) => s.ImageSource = v, () => viewmodel.UserImage);
            FindViewById<TextView>(Resource.Id.userName)
                .SetBinding((s, v) => s.Text = v, () => viewmodel.UserName);
            FindViewById<TextView>(Resource.Id.text)
                .SetBinding((s, v) => s.Text = v, () => viewmodel.Text)
                .SetTwoWay();

            sendButton.SetCommand(viewmodel.SendCommand);

            Bindings.EndScope();
        }

        protected override void OnResume()
        {
            base.OnResume();
            MessengerInstance.Register<WriteCommentViewModel.CloseMesssage>(
                this, _ => Finish());
        }
    }
}