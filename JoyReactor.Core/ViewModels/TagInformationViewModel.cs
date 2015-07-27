using System.Collections.Generic;
using System.Collections.ObjectModel;
using JoyReactor.Core.Model.DTO;
using JoyReactor.Core.Model.Helper;
using JoyReactor.Core.Model.Parser;
using JoyReactor.Core.ViewModels.Common;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using JoyReactor.Core.Model.Database;

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
            MessengerInstance.Register<Messages.SelectTagMessage>(
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
                yield return new TagViewModel(s);
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
            public string Title { get { return tag.Title; } }

            public string Image { get { return tag.BestImage; } }

            public ICommand SelectCommand { get; set; }

            Tag tag;

            public TagViewModel(Tag tag)
            {
                this.tag = tag;
                SelectCommand = new Command(
                    async () =>
                    {
                        await new TagRepository().InsertIfNotExistsAsync(tag);
                        var id = ID.DeserializeFromString(tag.TagId);
                        Messenger.Default.Send(new Messages.SelectTagMessage { Id = id });
                    });
            }
        }
    }
}