using Android.App;
using Android.Widget;
using JoyReactor.Core.Model;

namespace JoyReactor.Android.Platform
{
    public class PlatformMessageService : MessageService
    {
        public override void Show(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }
    }
}