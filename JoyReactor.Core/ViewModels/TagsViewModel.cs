using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;

namespace JoyReactor.Core.ViewModels
{
    public class TagsViewModel : ScopedViewModel
    {
        public ObservableCollection<TagItemViewModel> Tags { get; } = new ObservableCollection<TagItemViewModel>();

        public int SelectedTag { get { return Get<int>(); } set { Set(value); } }

        public override async void OnActivated()
        {
            base.OnActivated();
            PropertyChanged += TagsViewModel_PropertyChanged;

            var tags = await new TagRepository().GetAllAsync();
            var mainTags = tags.Where(s => (s.Flags & Tag.FlagShowInMain) == Tag.FlagShowInMain).ToList();
            OnTagsChanged(mainTags);
        }

        public override void OnDeactivated()
        {
            base.OnDeactivated();
            PropertyChanged -= TagsViewModel_PropertyChanged;
        }

        void TagsViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedTag" && SelectedTag >= 0)
                MessengerInstance.Send(new SelectTagMessage { Id = ID.Parser(Tags[SelectedTag].tag.TagId) });
        }

        void OnTagsChanged(List<Tag> tags)
        {
            var vms = tags
                .OrderBy(s => s.Title.ToUpper())
                .Select(s => new TagItemViewModel(s));
            Tags.ReplaceAll(vms);
        }

        public class TagItemViewModel : ViewModelBase
        {
            const string DefaultImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f6/Lol_question_mark.png/150px-Lol_question_mark.png";

            public ID TagId { get { return ID.Parser(tag.TagId); } }

            public string Title { get { return tag.Title; } }

            public string Image { get { return tag.BestImage ?? DefaultImage; } }

            internal Tag tag;

            public TagItemViewModel(Tag tag)
            {
                this.tag = tag;
            }
        }

        public class SelectTagMessage
        {
            public ID Id;
        }
    }
}