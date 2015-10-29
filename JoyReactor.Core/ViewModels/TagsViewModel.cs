using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using JoyReactor.Core.Model.Database;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.ViewModels.Common;

namespace JoyReactor.Core.ViewModels
{
    public class TagsViewModel : ScopedViewModel
    {
        public ObservableCollection<TagItemViewModel> Tags { get; } = new ObservableCollection<TagItemViewModel>();

        public int SelectedTag { get { return Get<int>(); } set { Set(value); } }

        public override void OnActivated()
        {
            base.OnActivated();

            AddPropertyListener(() => SelectedTag, 
                () =>
                {
                    if (SelectedTag >= 0)
                    {
                        var id = ID.DeserializeFromString(Tags[SelectedTag].tag.TagId);
                        MessengerInstance.Send(new Messages.SelectTagMessage { Id = id });
                    }
                });

            OnTagsChanged();
            MessengerInstance.Register<Messages.TagsChanged>(this, _ => OnTagsChanged());
        }

        async void OnTagsChanged()
        {
            var tags = await new TagRepository().GetAllAsync();
            var vms = tags
                .Where(s => (s.Flags & Tag.FlagShowInMain) == Tag.FlagShowInMain)
                .OrderBy(s => s.Title.ToUpper())
                .Select(s => new TagItemViewModel(s));
            Tags.ReplaceAll(vms);
        }

        public class TagItemViewModel : ViewModelBase
        {
            const string DefaultImage = "https://upload.wikimedia.org/wikipedia/commons/thumb/f/f6/Lol_question_mark.png/150px-Lol_question_mark.png";

            public ID TagId { get { return ID.DeserializeFromString(tag.TagId); } }

            public string Title { get { return tag.Title; } }

            public string Image { get { return tag.BestImage ?? DefaultImage; } }

            internal Tag tag;

            public TagItemViewModel(Tag tag)
            {
                this.tag = tag;
            }
        }
    }
}