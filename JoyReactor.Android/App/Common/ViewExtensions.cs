using Android.Views;

namespace JoyReactor.Android.App.Common
{
    public static class ViewExtensions
    {
        public static ViewStates ToViewStates(this bool instance)
        {
            return instance ? ViewStates.Visible : ViewStates.Gone;
        }
    }
}