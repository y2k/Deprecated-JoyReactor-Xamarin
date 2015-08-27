using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V7.Widget;
using Android.Views;
using JoyReactor.Android.App.Base;
using JoyReactor.Core.ViewModels;

namespace JoyReactor.Android.App.Posts
{
    [Activity(
        Label = "",
        Theme = "@style/AppTheme.Toolbar",
        ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait)]
    [MetaData("android.support.PARENT_ACTIVITY", Value = "net.itwister.joyreactor2.HomeActivity")]
    public class PostActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_post);
            SetSupportActionBar(FindViewById<Toolbar>(Resource.Id.toolbar));
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);

            if (savedInstanceState == null)
            {
                int id = Intent.GetIntExtra(Arg1, 0);
                SupportFragmentManager
					.BeginTransaction()
                    .Add(Resource.Id.container, PostFragment.NewFragment(id))
					.Commit();
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
            MessengerInstance.Register<PostViewModel.WriteCommentMessage>(
                this, m => StartActivity(new Intent(this, typeof(WriteCommentActivity))));
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.feedback_post, menu);
            return true;
        }

        public static Intent NewIntent(int id)
        {
            return BaseActivity.NewIntent(typeof(PostActivity), id);
        }
    }
}