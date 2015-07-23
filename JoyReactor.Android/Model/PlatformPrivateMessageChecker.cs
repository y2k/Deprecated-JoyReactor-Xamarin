using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using JoyReactor.Android.App;
using JoyReactor.Core.Model;

namespace JoyReactor.Android.Model
{
    public class PlatformPrivateMessageChecker : PrivateMessageChecker
    {
        const long RecheckPeriod = AlarmManager.IntervalHour;

        public override void StartCheck()
        {
            GetAlarmManager().SetInexactRepeating(
                AlarmType.ElapsedRealtime, 
                SystemClock.ElapsedRealtime(), 
                RecheckPeriod, 
                CreateServiceIntent());
        }

        public override void StopChecking()
        {
            GetAlarmManager().Cancel(CreateServiceIntent());
        }

        AlarmManager GetAlarmManager()
        {
            return (AlarmManager)GetContext().GetSystemService(Context.AlarmService);
        }

        PendingIntent CreateServiceIntent()
        {
            var i = new Intent(GetContext(), typeof(PrivateMessageCheckerService));
            return PendingIntent.GetService(GetContext(), 0, i, PendingIntentFlags.UpdateCurrent);
        }

        public override void ShowNotificationAboutNewMessages()
        {
            var i = new Intent(GetContext(), typeof(MessageActivity));
            var pi = PendingIntent.GetActivity(GetContext(), 0, i, PendingIntentFlags.UpdateCurrent);

            var n = new NotificationCompat.Builder(GetContext())
                .SetSmallIcon(Resource.Drawable.ic_notification)
                .SetTicker(GetContext().GetString(Resource.String.new_messages))
                .SetContentTitle(GetContext().GetString(Resource.String.new_messages))
                .SetContentText(GetContext().GetString(Resource.String.click_to_open))
                .SetContentIntent(pi)
                .Build();

            var nm = NotificationManagerCompat.From(GetContext());
            nm.Notify(0, n);
        }

        static Context GetContext()
        {
            return Application.Context;
        }
    }
}