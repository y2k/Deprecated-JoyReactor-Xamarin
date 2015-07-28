using System;
using System.Threading.Tasks;

namespace JoyReactor.Core.Model.Parser
{
    public class CreateCommentRequest
    {
        CreateCommentRequest()
        {
        }

        public Task ExecuteAsync()
        {
            throw new NotImplementedException();
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