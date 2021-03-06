﻿using JoyReactor.Core.Model.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JoyReactor.Core.Model.Parser
{
    public interface IProviderListStorage
    {
        Task AddPost(Post post);

        Task CommitAsync();
    }

    public interface IProviderStorage
    {
        Task SaveNewOrUpdatePostAsync(Post post);

        Task UpdateTagInformationAsync(ID id, string image, int nextPage, bool hasNextPage);

        Task ReplacePostAttachments(string postId, List<Attachment> attachments);

        Task RemovePostComments(string postId);

        Task SaveNewPostCommentAsync(string postId, int parrentCommentId, Comment comment, string[] attachments);

        Task SaveNewOrUpdateProfileAsync(Profile profile);

        Task ReplaceCurrentUserReadingTagsAsync(IEnumerable<string> readingTags);

        Task<int> GetNextPageForTagAsync(ID id);

        Task SaveRelatedPostsAsync(string postId, List<RelatedPost> posts);

        Task SaveLinkedTagsAsync(ID id, string groupName, ICollection<Tag> tags);

        Task RemoveLinkedTagAsync(ID id);
    }

    public interface IProviderAuthStorage
    {
        [Obsolete]
        Task<string> GetCurrentUserNameAsync();

        Task<IDictionary<string, string>> GetCookiesAsync();

        Task SaveCookieToDatabaseAsync(string username, IDictionary<string, string> cookies);
    }
}