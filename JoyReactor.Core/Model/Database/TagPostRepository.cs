﻿using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.DTO;

namespace JoyReactor.Core.Model.Database
{
    class TagPostRepository : Repository
    {
        public Task<List<TagPost>> GetAllAsync(int tagId)
        {
            return Connection.QueryAsync<TagPost>($"SELECT * FROM tag_post WHERE TagId = ?", tagId);
        }

        public Task RemoveAsync(int tagId)
        {
            return Connection.ExecuteAsync("DELETE FROM tag_post WHERE TagId = ?", tagId);
        }

        public Task<int> AddAsync(TagPost item)
        {
            return Connection.InsertAsync(item);
        }
    }
}