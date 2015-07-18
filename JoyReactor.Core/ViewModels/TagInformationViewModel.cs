using System.Collections.Generic;
using System.Collections.ObjectModel;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.Model.Parser;

namespace JoyReactor.Core.ViewModels
{
    public class TagInformationViewModel : ScopedViewModel
    {
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        public TagInformationViewModel()
        {
            ChangeCurrentTag(ID.Factory.New(ID.IdConst.ReactorGood));
        }

        public override void OnActivated()
        {
            base.OnActivated();
            MessengerInstance.Register<TagsViewModel.SelectTagMessage>(
                this, m => ChangeCurrentTag(m.Id));
        }

        public async void ChangeCurrentTag(ID currentTagId)
        {
            Items.Clear();

            var request = new TagInformationRequest(currentTagId);
            await request.ComputeAsync();

            foreach (var s in request.LinkedTags)
            {
                Items.Add(new GroupViewModel { Title = s.Title });
                Items.AddRange(ConvertToViewModels(s.Tags));
            }
        }

        IEnumerable<TagViewModel> ConvertToViewModels(List<Tag> tags)
        {
            foreach (var s in tags)
                yield return new TagViewModel { Title = s.Title, Image = s.BestImage };
        }

        public abstract class Item
        {
        }

        public class GroupViewModel : Item
        {
            public string Title { get; set; }
        }

        public class TagViewModel : Item
        {
            public string Title { get; set; }

            public string Image { get; set; }
        }
    }
}