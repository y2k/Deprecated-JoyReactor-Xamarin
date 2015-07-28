using System;
using System.Threading.Tasks;
using System.Windows.Input;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.Parser;

namespace JoyReactor.Core.ViewModels
{
    public class WriteCommentViewModel : ScopedViewModel
    {
        public string UserImage { get { return Get<string>(); } set { Set(value); } }

        public string UserName { get { return Get<string>(); } set { Set(value); } }

        public string Text { get { return Get<string>(); } set { Set(value); } }

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        public bool CanSend { get { return Get<bool>(); } set { Set(value); } }

        public ICommand SendCommand { get; set; }

        public WriteCommentViewModel()
        {
            SendCommand = new Command(
                async () =>
                {
                    if (string.IsNullOrEmpty(Text))
                        return;
                    IsBusy = true;
                    await SendCommandAsync();
                    IsBusy = false;
                });

            AddPropertyListener(() => Text, () => CanSend = !string.IsNullOrEmpty(Text));
            Initialize();
        }

        async void Initialize()
        {
            var profile = await new ProfileRepository().GetCurrentAsync();
            UserImage = profile.UserImage;
            UserName = profile.UserName;
        }

        async Task SendCommandAsync()
        {
            try
            {
                var post = await new PostRepository().GetAsync(-1);
                CreateCommentRequest request;
                if (true)
                    request = CreateCommentRequest.ForPost(post.PostId);
                else
                {
                    var comment = await new CommentRepository().GetCommentAsync(-1);
                    request = CreateCommentRequest.ForComment(post.PostId, "");
                }

                await request.ExecuteAsync();
                MessengerInstance.Send(new CloseMesssage());
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public class CloseMesssage
        {
        }

        public class Argument
        {
            int postId;
            int commentId;
        }
    }
}