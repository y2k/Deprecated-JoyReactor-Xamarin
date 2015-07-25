using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.ViewModels
{
    public class MessagesViewModel : ScopedViewModel
    {
        public ObservableCollection<PrivateMessage> Messages { get; } = new ObservableCollection<PrivateMessage>();

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        public string NewMessage { get { return Get<string>(); } set { Set(value); } }

        public RelayCommand CreateMessageCommand { get; set; }

        string username;

        public MessagesViewModel()
        {
            CreateMessageCommand = new Command(CreateNewMessage);
        }

        async Task CreateNewMessage()
        {
            IsBusy = true;

            var service = ServiceLocator.Current.GetInstance<IMessageService>();
            await service.SendMessage(username, NewMessage);
            NewMessage = null;

            await SwitchUser(username);

            IsBusy = false;
        }

        public override void OnActivated()
        {
            base.OnActivated();
            MessengerInstance.Register<SelectThreadMessage>(this, m => SwitchUser(m.Username));
        }

        async Task SwitchUser(string username)
        {
            this.username = username;

            IsBusy = true;
            Messages.Clear();

            var messages = await new MessageRepository().GetMessagesAsync(username);
            Messages.ReplaceAll(messages.AsEnumerable().Reverse());

            IsBusy = false;
        }

        public class SelectThreadMessage
        {
            public string Username { get; set; }
        }

        internal interface IMessageService
        {
            Task SendMessage(string username, string message);
        }
    }
}