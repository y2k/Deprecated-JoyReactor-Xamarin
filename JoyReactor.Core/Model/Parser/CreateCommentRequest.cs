using System;
using System.Threading.Tasks;
using JoyReactor.Core.Model.Web;
using Microsoft.Practices.ServiceLocation;
using JoyReactor.Core.Model.Database;
using System.Text.RegularExpressions;
using System.IO;
using PCLStorage;

namespace JoyReactor.Core.Model.Parser
{
    public class CreateCommentRequest
    {
        WebDownloader client = ServiceLocator.Current.GetInstance<WebDownloader>();

        string postId;
        string commentId;

        string commentText;
        string commentPicture;

        CreateCommentRequest()
        {
        }

        public async Task ExecuteAsync()
        {
            var request = new RequestParams()
                .SetContentTypeMultipart()
                .AddForm("parent_id", commentId ?? "0")
                .AddForm("post_id", postId)
                .AddForm("token", await GetToken())
                .AddForm("comment_text", commentText);

            if (commentPicture != null)
                request.AddData("comment_picture", await GetPicture(), Path.GetFileName(commentPicture), "image/jpeg");

            await client.ExecuteAsync(
                new Uri("http://joyreactor.cc/post_comment/create"), request);
        }

        async Task<string> GetToken()
        {
            var page = await client.GetTextAsync(
                           new Uri("http://joyreactor.cc/donate"),
                           new RequestParams { Cookies = await User.Current.GetCookesAsync() });
            return Regex.Match(page, "var token = '(.+?)'").Groups[1].Value;
        }

        async Task<byte[]> GetPicture()
        {
            var file = await FileSystem.Current.LocalStorage.GetFileAsync(commentPicture);
            var buf = new MemoryStream();
            using (var z = await file.OpenAsync(FileAccess.Read))
                await z.CopyToAsync(buf);
            return buf.ToArray();
        }

        public static CreateCommentRequest ForPost(string postId)
        {
            return new CreateCommentRequest();
        }

        public static CreateCommentRequest ForComment(string postId, string commentId)
        {
            return new CreateCommentRequest();
        }
    }
}