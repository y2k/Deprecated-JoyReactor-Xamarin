using System.Threading.Tasks;
using JoyReactor.Core.Model.Parser;

namespace JoyReactor.Core.Model
{
    public abstract class PrivateMessageChecker
    {
        public static PrivateMessageChecker Instance { get; set; }

        public abstract void StartCheck();

        public abstract void StopChecking();

        public abstract void ShowNotificationAboutNewMessages();

        public async Task UpdateAsync()
        {
            var request = new CheckNewPrivateMessageRequest();
            try
            {
                await request.RequestAsync();
            }
            catch
            {
            }

            if (request.HasNewPrivateMessages)
                ShowNotificationAboutNewMessages();
        }

        public async void Initialize()
        {
            if (await User.Current.IsAuthorizedAsync())
                StartCheck();
        }
    }
}