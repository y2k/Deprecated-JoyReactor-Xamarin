using System.Collections.ObjectModel;
using Android.OS;
using Android.Views;
using Android.Widget;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.App.Common;
using JoyReactor.Android.Widget;
using JoyReactor.Core.ViewModels;

namespace JoyReactor.Android.App.Home
{
    public class RightMenuFragment : BaseFragment
    {
        ListView list;
        TagInformationViewModel viewModel;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            viewModel = Scope.New<TagInformationViewModel>();
        }

        public override void OnActivityCreated(Bundle savedInstanceState)
        {
            base.OnActivityCreated(savedInstanceState);

            list.Adapter = new Adapter(viewModel.Items);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            View v = inflater.Inflate(Resource.Layout.fragment_left_menu, null);
            list = v.FindViewById<ListView>(Resource.Id.list);
            return v;
        }

        public class Adapter : BaseAdapter<TagInformationViewModel.GroupViewModel>
        {
            public ObservableCollection<TagInformationViewModel.Item> source;

            public Adapter(ObservableCollection<TagInformationViewModel.Item> source)
            {
                this.source = source;
                source.CollectionChanged += (sender, e) => NotifyDataSetChanged();
            }

            public override int Count
            {
                get { return source == null ? 0 : source.Count; }
            }

            public override int ViewTypeCount
            {
                get { return 2; }
            }

            public override int GetItemViewType(int position)
            {
                return source[position] is TagInformationViewModel.GroupViewModel ? 0 : 1;
            }

            public override View GetView(int position, View convertView, ViewGroup parent)
            {
                View view;
                var inflater = LayoutInflater.From(parent.Context);
                if (GetItemViewType(position) == 0)
                {
                    view = convertView ?? inflater.Inflate(Resource.Layout.item_tag_group, parent, false);
                    var i = source[position] as TagInformationViewModel.GroupViewModel;
                    view.FindViewById<TextView>(Resource.Id.title).Text = i.Title.ToLower();
                }
                else
                {
                    view = convertView ?? inflater.Inflate(Resource.Layout.item_linked_tag, parent, false);
                    var i = source[position] as TagInformationViewModel.TagViewModel;
                    view.FindViewById<TextView>(Resource.Id.title).Text = i.Title;
                    view.FindViewById<WebImageView>(Resource.Id.icon).ImageSource = i.Image;
                    view.FindViewById(Resource.Id.button).SetCommand(i.SelectCommand);
                }
                return view;
            }

            public override long GetItemId(int position)
            {
                return position;
            }

            public override TagInformationViewModel.GroupViewModel this [int index]
            {
                get { return null; }
            }
        }
    }
}