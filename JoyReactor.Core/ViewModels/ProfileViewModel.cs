using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JoyReactor.Core.Model;
using JoyReactor.Core.Model.Common;

namespace JoyReactor.Core.ViewModels
{
    public class ProfileViewModel : ScopedViewModel
    {
        public bool IsLoading { get { return Get<bool>(); } set { Set(value); } }

        public string Avatar { get { return Get<string>(); } set { Set(value); } }

        public string UserName { get { return Get<string>(); } set { Set(value); } }

        public float Rating { get { return Get<float>(); } set { Set(value); } }

        public int Stars { get { return Get<int>(); } set { Set(value); } }

        public float NextStarProgress { get { return Get<float>(); } set { Set(value); } }

        public RelayCommand LogoutCommand { get; set; }

        public ProfileViewModel()
        {
            LogoutCommand = new Command(Logout);
            Initialize();
        }

        async void Initialize()
        {
            IsLoading = true;
            try
            {
                var profile = await new ProfileService().GetMyProfile();
                UserName = profile.UserName;
                Avatar = profile.UserImage;
                Rating = profile.Rating;
                Stars = profile.Stars;
                NextStarProgress = profile.NextStarProgress;
            }
            catch (NotLogedException)
            {
                NavigateToLogin();
            }
            IsLoading = false;
        }

        async Task Logout()
        {
            await new ProfileService().Logout();
            PrivateMessageChecker.Instance.StopChecking();
            NavigateToLogin();
        }

        void NavigateToLogin()
        {
            MessengerInstance.Send(new NavigateToLoginMessage());
        }

        public class NavigateToLoginMessage
        {
        }
    }
}