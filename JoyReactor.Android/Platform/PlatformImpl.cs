using System;
using Android.App;

namespace JoyReactor.Android.Platform
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

        static global::Android.Content.Context GetContext()
        {
            return Application.Context;
        }
    }
}