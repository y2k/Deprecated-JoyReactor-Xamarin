using System;
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
    public class MessagesViewModel : ViewModel
    {
        public ObservableCollection<PrivateMessage> Messages { get; } = new ObservableCollection<PrivateMessage>();

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        public string NewMessage { get { return Get<string>(); } set { Set(value); } }

        public RelayCommand CreateMessageCommand { get; set; }

        string currentUserName;

        public MessagesViewModel()
        {
            MessengerInstance.Register<SelectThreadMessage>(this, m => SwitchUser(m.Username));
            CreateMessageCommand = new Command(CreateNewMessage);
        }

        async Task CreateNewMessage()
        {
            IsBusy = true;
            await ServiceLocator.Current.GetInstance<IMessageService>()
                .SendMessage(currentUserName, NewMessage);
            NewMessage = null;
            IsBusy = false;
        }

        async void SwitchUser(string username)
        {
            Messages.Clear();
            IsBusy = true;

            currentUserName = username;

            var messages = await new MessageRepository().GetMessagesAsync(username);
            OnNext(messages);
        }

        void OnNext(List<PrivateMessage> messages)
        {
            // TODO
            Messages.ReplaceAll(messages.AsEnumerable().Reverse());
            IsBusy = false;
        }

        void OnError(Exception error)
        {
            // TODO
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