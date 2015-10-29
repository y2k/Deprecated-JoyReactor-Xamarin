using Android.OS;
using Android.Support.V4.Widget;
using Android.Support.V7.Widget;
using Android.Views;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.App.Common;
using JoyReactor.Android.Widget;
using JoyReactor.Core.ViewModels;
using JoyReactor.Core.ViewModels.Common;

namespace JoyReactor.Android.App.Home
{
    public class FeedFragment : BaseFragment
    {
        FeedViewModel viewmodel;
        FeedRecyclerView list;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            RetainInstance = true;
            viewmodel = Scope.New<FeedViewModel>();

            MessengerInstance.Register<Messages.SelectTagMessage>(this, _ => list.ResetScrollToTop());
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.fragment_feed, null);
            Bindings.BeginScope(viewmodel);

            list = view.FindViewById<FeedRecyclerView>(Resource.Id.list);
            list.SetAdapter(new FeedAdapter(viewmodel.Posts, viewmodel));

            var refresher = view.FindViewById<SwipeRefreshLayout>(Resource.Id.refresher);
            refresher.SetCommand(viewmodel.RefreshCommand);
            refresher.SetBinding((s, v) => s.Refreshing = v, () => viewmodel.IsBusy);

            var applyButton = view.FindViewById<ReloadButton>(Resource.Id.apply);
            applyButton.Command = viewmodel.ApplyCommand;
            applyButton.SetBinding(ChangeVisible, () => viewmodel.HasNewItems);

            view.FindViewById(Resource.Id.error)
                .SetBinding((s, v) => s.SetVisibility(v != FeedViewModel.ErrorType.NotError), () => viewmodel.Error);

            var toolbar = view.FindViewById<Toolbar>(Resource.Id.toolbar);
            ((HomeActivity)Activity).SetSupportActionBar(toolbar);
            toolbar.SetNavigationIcon(Resource.Drawable.ic_menu_white_24dp);

            Bindings.EndScope();
            return view;
        }

        async void ChangeVisible(ReloadButton s, bool visible)
        {
            if (visible)
            {
                s.SetVisibility(visible);
                await s.ViewTreeObserver.WaitPreDrawAsync();
                s.Alpha = 0;
                s.TranslationY = s.Height;
                s.Animate().TranslationY(0).Alpha(1);
            }
            else
            {
                s.Alpha = 1;
                await s.Animate().Alpha(0).TranslationY(s.Height).EndAsync();
                s.SetVisibility(visible);
            }
        }
    }
}