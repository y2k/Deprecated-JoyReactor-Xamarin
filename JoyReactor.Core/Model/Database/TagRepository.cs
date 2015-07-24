using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JoyReactor.Core.Model.DTO;

namespace JoyReactor.Core.Model.Database
{
    class TagRepository : Repository<Tag>
    {
        public async Task<Tag> GetAsync(string tagId)
        {
            return (await Connection.QueryAsync<Tag>("SELECT * FROM tags WHERE TagId = ?", tagId)).First();
        }

        internal Task<int> CountAsync(string id)
        {
            return Connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM tags WHERE TagId = ?", id);
        }

        public async Task InsertIfNotExistsAsync(Tag tag)
        {
            var count = (await Connection.QueryAsync<Tag>("SELECT * FROM tags WHERE TagId = ?", tag.Id)).Count();
            if (count == 0)
                await InsertAsync(tag);
        }

        public Task<List<Tag>> GetAllAsync()
        {
            return Connection.QueryAsync<Tag>("SELECT * FROM tags");
        }
    }
}