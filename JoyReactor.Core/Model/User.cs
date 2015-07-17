using System;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Database;

namespace JoyReactor.Core.Model
{
    public class User
    {
        public static User Current = new User();

        public async Task<bool> IsAuthorizedAsync()
        {
            try
            {
                var profile = await new ProfileRepository().GetCurrentAsync();
                return profile != null;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}