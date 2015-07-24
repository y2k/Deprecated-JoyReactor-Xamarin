using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model.Messages
{
    public class MessageService : MessagesViewModel.IMessageService
    {
        readonly IStorage storage = ServiceLocator.Current.GetInstance<IStorage>();

        public static event EventHandler MessagesChanged;

        async Task<List<MessageThreadItem>> GetThreadsAsync()
        {
            await new MessageFetcher().FetchAsync();
            return await storage.GetThreadsWithAdditionInformationAsync();
        }

        public async Task SendMessage(string username, string message)
        {
            await new ReactorMessageParser().SendMessageToUser(username, message);
            await storage.PutMessageAsync(
                username,
                new PrivateMessage
                {
                    Message = message,
                    Created = DateTime.Now,
                    Mode = PrivateMessage.ModeOutbox
                });
            MessagesChanged?.Invoke(null, null);
        }

        public interface IStorage
        {
            Task<List<MessageThreadItem>> GetThreadsWithAdditionInformationAsync();

            Task<List<PrivateMessage>> GetMessagesAsync(string username);

            Task PutMessageAsync(string username, PrivateMessage message);
        }
    }
}