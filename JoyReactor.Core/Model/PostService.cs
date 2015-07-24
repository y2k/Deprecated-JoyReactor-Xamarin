using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.ViewModels;
using Microsoft.Practices.ServiceLocation;

namespace JoyReactor.Core.Model
{
    public class PostService : CreateTagViewModel.IPostService
    {
        IStorage storage = ServiceLocator.Current.GetInstance<IStorage>();

        internal static event EventHandler PostChanged;

        int postId;

        public PostService()
        {
        }

        public PostService(int postId)
        {
            this.postId = postId;
        }

        async Task<List<Comment>> GetComments(int commentId)
        {
            var comments = await storage.GetChildCommentsAsync(postId, commentId);
            if (commentId != 0)
                comments.Insert(0, await storage.GetCommentAsync(commentId));
            return comments;
        }

        async Task<Post> GetPostAsync()
        {
            var post = await storage.GetPostAsync(postId);
            post.RelatedPosts = await storage.GetRelatedPostsAsync(postId);
            post.Attachments = await storage.GetAttachmentsAsync(postId);
            return post;
        }

        async void SyncPost()
        {
            var post = await storage.GetPostAsync(postId);
            await new PostRequest(post.PostId).ComputeAsync();
            PostChanged?.Invoke(null, null);
        }

        [Obsolete]
        public async Task CreateTagAsync(string name)
        {
            await storage.CreateMainTagAsync(name);
            TagCollectionModel.InvalidateTagCollection();
        }

        internal interface IStorage
        {
            Task<Post> GetPostAsync(int postId);

            Task<List<Attachment>> GetAttachmentsAsync(int postId);

            Task CreateMainTagAsync(string name);

            Task<List<RelatedPost>> GetRelatedPostsAsync(int postId);

            Task<List<Comment>> GetChildCommentsAsync(int postId, int commentId);

            Task<Comment> GetCommentAsync(int commentId);
        }
    }
}