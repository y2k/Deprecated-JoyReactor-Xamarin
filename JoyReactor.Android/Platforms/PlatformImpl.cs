using System;
using Android.App;
using Android.Widget;

namespace JoyReactor.Android.Platforms
{
    public class PlatformImpl : JoyReactor.Core.Model.Platform
    {
        public override Version GetVersion()
        {
            var app = GetContext();
            var info = app.PackageManager.GetPackageInfo(app.PackageName, 0);
            return new Version(info.VersionName);
        }

        public override string GetPlatformString(string key)
        {
            var context = GetContext();
            var resId = context.Resources.GetIdentifier(key, "string", context.PackageName);
            if (resId == 0)
                throw new ArgumentException($"Not found StringId for {nameof(key)} = {key}");
            return context.GetString(resId);
        }

        public override void ShowMessage(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        static global::Android.Content.Context GetContext()
        {
            return Application.Context;
        }
    }
}