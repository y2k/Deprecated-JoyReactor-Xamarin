using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.Model.Web;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model.Parser
{
    internal class CheckNewPrivateMessageRequest
    {
        public bool HasNewPrivateMessages { get; private set; }

        public async Task RequestAsync()
        {
            var downloader = ServiceLocator.Current.GetInstance<WebDownloader>();
            var doc = await downloader.GetTextAsync(
                          new Uri("http://joyreactor.cc/ads"), 
                          new RequestParams { Cookies = await GetUserCookies() });

            // <a href="/private/list" class="icon_mail new" title="Мои сообщения">
            HasNewPrivateMessages = doc.Contains("class=\"icon_mail new\"");
        }

        Task<IDictionary<string, string>> GetUserCookies()
        {
            var authStorage = ServiceLocator.Current.GetInstance<IProviderAuthStorage>();
            return authStorage.GetCookiesAsync();
        }
    }
}