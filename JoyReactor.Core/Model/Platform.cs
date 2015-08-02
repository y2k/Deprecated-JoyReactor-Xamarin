using System;

namespace JoyReactor.Core.Model
{
    public abstract class Platform
    {
        public static Platform Instance { get; set; }

        public abstract Version GetVersion();

        public abstract string GetPlatformString(string key);

        public abstract void Show(string message);
    }
}