using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        public async void Initialize()
        {
            IsBusy = true;

            await new MessageFetcher().FetchAsync();
            var msgs = await new MessageRepository().GetThreadsWithAdditionInformationAsync();
            OnNext(msgs);
        }

        void OnNext(List<MessageThreadItem> threads)
        {
            IsBusy = false;
            Threads.ReplaceAll(threads);
        }

        void OnError(Exception e)
        {
            // TODO
            IsBusy = false;
        }

        public override void OnActivated()
        {
            base.OnActivated();
            PropertyChanged += MessageThreadsViewModel_PropertyChanged;
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated();
            PropertyChanged -= MessageThreadsViewModel_PropertyChanged;
        }

        void MessageThreadsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == GetPropertyName(() => SelectedIndex) && SelectedIndex >= 0)
                MessengerInstance.Send(new MessagesViewModel.SelectThreadMessage { Username = Threads[SelectedIndex].UserName });
        }
    }
}