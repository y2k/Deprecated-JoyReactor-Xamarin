using Android.App;
using Android.Content;
using Android.Support.V4.App;
using JoyReactor.Android.App;

namespace JoyReactor.Android.Model
{
    public class PrivateMessageChecker : JoyReactor.Core.Model.PrivateMessageChecker
    {
        #if DEBUG
        const long RecheckPeriod = 60 * 1000; // 1 minutes
        #else
        const long RecheckPeriod = AlarmManager.IntervalHour;
        #endif

        static PrivateMessageChecker()
        {
            Instance = new PrivateMessageChecker();
        }

        public override void StartCheck()
        {
            var i = new Intent(GetContext(), typeof(PrivateMessageCheckerService));
            var pi = PendingIntent.GetService(GetContext(), 0, i, PendingIntentFlags.UpdateCurrent);

            var am = (AlarmManager)GetContext().GetSystemService(Context.AlarmService);
            am.SetInexactRepeating(AlarmType.ElapsedRealtime, 0, RecheckPeriod, pi);
        }

        public override void ShowNotificationAboutNewMessages()
        {
            var i = new Intent(GetContext(), typeof(MessageActivity));
            var pi = PendingIntent.GetActivity(GetContext(), 0, i, PendingIntentFlags.UpdateCurrent);

            var n = new NotificationCompat.Builder(GetContext())
                .SetSmallIcon(Resource.Mipmap.ic_launcher)
                .SetTicker(GetContext().GetString(Resource.String.new_messages))
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