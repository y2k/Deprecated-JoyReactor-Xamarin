using System;
using System.Threading.Tasks;
using System.Windows.Input;
using JoyReactor.Core.Model.Database;

namespace JoyReactor.Core.ViewModels
{
    public class WriteCommentViewModel : ScopedViewModel
    {
        public string UserImage { get { return Get<string>(); } set { Set(value); } }

        public string UserName { get { return Get<string>(); } set { Set(value); } }

        public string Text { get { return Get<string>(); } set { Set(value); } }

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        public bool CanSend { get { return Get<bool>(); } set { Set(value); } }

        public ICommand SendCommand { get; set; }

        public WriteCommentViewModel()
        {
            SendCommand = new Command(
                async () =>
                {
                    if (string.IsNullOrEmpty(Text))
                        return;
                    IsBusy = true;
                    await SendCommandAsync();
                    IsBusy = false;
                });

            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Text))
                    CanSend = !string.IsNullOrEmpty(Text);
            };

            Initialize();
        }

        async void Initialize()
        {
            var profile = await new ProfileRepository().GetCurrentAsync();
            UserImage = profile.UserImage;
            UserName = profile.UserName;
        }

        async Task SendCommandAsync()
        {
            try
            {
                await Task.Delay(2000);
                MessengerInstance.Send(new CloseMesssage());
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public class CloseMesssage
        {
        }
    }
}