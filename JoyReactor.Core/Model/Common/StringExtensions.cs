namespace JoyReactor.Core.Model.Common
{
    public static class StringExtensions
    {
        public static string Translate(this string instance)
        {
            return Platform.Instance.GetPlatformString(instance);
        }
    }
}