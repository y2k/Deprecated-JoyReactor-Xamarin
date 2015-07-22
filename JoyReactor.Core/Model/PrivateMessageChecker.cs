using System.Threading.Tasks;
using JoyReactor.Core.Model.Parser;

namespace JoyReactor.Core.Model
{
    public abstract class PrivateMessageChecker
    {
        public static PrivateMessageChecker Instance { get; set; }

        public abstract void StartCheck();

        public abstract void ShowNotificationAboutNewMessages();

        public async Task UpdateAsync()
        {
            var request = new CheckNewPrivateMessageRequest();
            await request.RequestAsync();

            if (request.HasNewPrivateMessages)
                ShowNotificationAboutNewMessages();
        }
    }
}