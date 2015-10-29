using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Parser;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model
{
    public class TagCollectionModel
    {
        internal static event EventHandler InvalidateEvent;

        public async Task UpdateTagAsync(ID id, PostCollectionRequest request)
        {
            var tag = await new TagRepository().GetAsync(id.SerializeToString());
            tag.BestImage = request.TagImage;
            await new TagRepository().UpdateAsync(tag);
            await InvalidateTagCollectionAsync();
        }

        [Obsolete]
        public static void InvalidateTagCollection()
        {
            InvalidateEvent?.Invoke(null, null);
        }

        public static Task InvalidateTagCollectionAsync()
        {
            return Task.Run(() => InvalidateEvent?.Invoke(null, null));
        }

        [Obsolete]
        AsyncSQLiteConnection connection = ServiceLocator.Current.GetInstance<AsyncSQLiteConnection>();

        Task<List<Tag>> DoGetSubscriptionsAsync()
        {
            return connection.QueryAsync<Tag>(
                "SELECT * FROM tags WHERE Flags & ? != 0", 
                Tag.FlagShowInMain);
        }

        public interface Storage
        {
            Task<ICollection<TagGroup>> GetLinkedTagsAsync(ID id);
        }
    }
}