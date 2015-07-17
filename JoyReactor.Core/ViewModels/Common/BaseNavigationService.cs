namespace JoyReactor.Core.ViewModels.Common
{
    public abstract class BaseNavigationService
    {
        public static BaseNavigationService Instance { get; private set; }

        public static void Reset(BaseNavigationService service)
        {
            Instance = service ?? new Stub();
        }

        public abstract T GetArgument<T>();

        public abstract void ImageFullscreen(string imageUrl);

        class Stub : BaseNavigationService
        {
            public override T GetArgument<T>()
            {
                return default(T);
            }

            public override void ImageFullscreen(string imageUrl)
            {
                // Ignore
            }
        }
    }
}