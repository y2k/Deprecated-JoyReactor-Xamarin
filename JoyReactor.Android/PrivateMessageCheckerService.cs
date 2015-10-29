using Android.App;
using Android.Content;
using Android.OS;
using JoyReactor.Core.Model;

namespace JoyReactor.Android
{
    [Service(Label = "@string/private_message_service_label")]
    public class PrivateMessageCheckerService : Service
    {
        #region implemented abstract members of Service

        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        #endregion

        [System.Obsolete]
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            ExecuteAsync();
            return StartCommandResult.NotSticky;
        }

        async void ExecuteAsync()
        {
            await PrivateMessageChecker.Instance.UpdateAsync();
            StopSelf();
        }
    }
}