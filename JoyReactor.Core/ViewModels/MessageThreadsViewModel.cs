using System.Collections.Generic;
using System.Collections.ObjectModel;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.Model.Messages;

namespace JoyReactor.Core.ViewModels
{
    public class MessageThreadsViewModel : ScopedViewModel
    {
        public ObservableCollection<MessageThreadItem> Threads { get; } = new ObservableCollection<MessageThreadItem>();

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        public int SelectedIndex { get { return Get<int>(); } set { Set(value); } }

        public MessageThreadsViewModel()
        {
            SelectedIndex = -1;
            AddPropertyListener(() => SelectedIndex, () =>
                {
                    var message = new MessagesViewModel.SelectThreadMessage
                    {
                        Username = Threads[SelectedIndex].UserName
                    };
                    if (SelectedIndex >= 0)
                        MessengerInstance.Send(message);
                });

            Initialize();
        }

        async void Initialize()
        {
            IsBusy = true;

            await new MessageFetcher().FetchAsync();
            var threads = await new MessageRepository().GetThreadsWithAdditionInformationAsync();
            Threads.ReplaceAll(threads);

            IsBusy = false;
        }
    }
}