using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.Parser;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model
{
    public class User
    {
        public static User Current = new User();

        public async Task<bool> IsAuthorizedAsync()
        {
            try
            {
                return await GetProfile() != null;
            }
            catch
            {
                return false;
            }
        }

        public Task<IDictionary<string,string>> GetCookesAsync()
        {
            return ServiceLocator.Current
                .GetInstance<IProviderAuthStorage>()
                .GetCookiesAsync();
        }

        Task<JoyReactor.Core.Model.DTO.Profile> GetProfile()
        {
            return new ProfileRepository().GetCurrentAsync();
        }
    }
}