using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using JoyReactor.Core.Model;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.ViewModels.Common;

namespace JoyReactor.Core.ViewModels
{
    public class CreateTagViewModel : ViewModel
    {
        #region Properties

        public string Name { get { return Get<string>(); } set { Set(value); } }

        public bool NameError { get { return Get<bool>(); } set { Set(value); } }

        public bool IsBusy { get { return Get<bool>(); } set { Set(value); } }

        #endregion

        public RelayCommand CreateCommand { get; set; }

        public CreateTagViewModel()
        {
            CreateCommand = new RelayCommand(OnCreateTag);
        }

        void OnCreateTag()
        {
            if (ValidTagName())
                CreateTag();
        }

        bool ValidTagName()
        {
            NameError = string.IsNullOrWhiteSpace(Name);
            return !NameError;
        }

        async void CreateTag()
        {
            IsBusy = true;

            var tag = new Tag
            {
                Title = Name.Trim(),
                TagId = ID.Factory.NewTag(Name.Trim().ToLower()).SerializeToString(),
                Flags = Tag.FlagShowInMain,
            };
            await new TagImageRequest(tag).LoadAsync();
            if (Uri.IsWellFormedUriString(tag.BestImage, UriKind.Absolute))
            {
                await new TagRepository().InsertIfNotExistsAsync(tag);
                await TagCollectionModel.InvalidateTagCollectionAsync();

                MessengerInstance.Send(new CloseMessage());
                MessengerInstance.Send(new Messages.TagsChanged());
            }

            IsBusy = false;
        }

        public class CloseMessage
        {
        }

        internal interface IPostService
        {
            Task CreateTagAsync(string tagName);
        }
    }
}