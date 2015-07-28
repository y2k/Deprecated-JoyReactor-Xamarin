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

            var sendButton = FindViewById(Resource.Id.send);
            sendButton.SetCommand(viewmodel.SendCommand);
            Bindings
                .Add(viewmodel, () => viewmodel.CanSend)
                .WhenSourceChanges(() => sendButton.Enabled = viewmodel.CanSend);
            var progress = FindViewById(Resource.Id.progress);
            Bindings
                .Add(viewmodel, () => viewmodel.IsBusy)
                .WhenSourceChanges(() => progress.Visibility = viewmodel.IsBusy ? ViewStates.Visible : ViewStates.Gone);
            var userImage = FindViewById<WebImageView>(Resource.Id.userImage);
            Bindings
                .Add(viewmodel, () => viewmodel.UserImage)
                .WhenSourceChanges(() => userImage.ImageSource = viewmodel.UserImage);
            var userName = FindViewById<TextView>(Resource.Id.userName);
            Bindings
                .Add(viewmodel, () => viewmodel.UserName)
                .WhenSourceChanges(() => userName.Text = viewmodel.UserName);
            var text = FindViewById<TextView>(Resource.Id.text);
            text.TextChanged += (sender, e) => viewmodel.Text = text.Text;
        }

        protected override void OnResume()
        {
            base.OnResume();
            MessengerInstance.Register<WriteCommentViewModel.CloseMesssage>(
                this, _ => Finish());
        }
    }
}