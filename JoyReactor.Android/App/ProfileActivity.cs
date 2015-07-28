using Android.App;
using Android.OS;
using Android.Views;
using Android.Widget;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.Widget;
using JoyReactor.Core.ViewModels;
using Messenger = GalaSoft.MvvmLight.Messaging.Messenger;
using JoyReactor.Android.App.Common;

namespace JoyReactor.Android.App
{
    [Activity(
        Label = "@string/profile",
        ScreenOrientation = global::Android.Content.PM.ScreenOrientation.Portrait,
        WindowSoftInputMode = SoftInput.AdjustResize)]			
    [MetaData("android.support.PARENT_ACTIVITY", Value = "net.itwister.joyreactor2.HomeActivity")]
    public class ProfileActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentViewForFragment();
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            if (savedInstanceState == null)
                SetRootFragment(new ProfileFragment());

            MessengerInstance.Register<LoginViewModel.NavigateToProfileMessage>(
                this, m => SetRootFragment(new ProfileFragment()));
            MessengerInstance.Register<ProfileViewModel.NavigateToLoginMessage>(
                this, m => SetRootFragment(new LoginFragment()));
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            Messenger.Default.Unregister(this);
        }

        public class ProfileFragment : BaseFragment
        {
            ProfileViewModel viewmodel;

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                RetainInstance = true;
                viewmodel = Scope.New<ProfileViewModel>();
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = inflater.Inflate(Resource.Layout.fragment_profile, container, false);

                Bindings.BeginScope(viewmodel);

                view.FindViewById(Resource.Id.progress)
                    .SetBinding((s, v) => s.Visibility = v.ToViewStates(), () => viewmodel.IsLoading);

                view.FindViewById<TextView>(Resource.Id.username)
                    .SetBinding((s, v) => s.Text = v, () => viewmodel.UserName);

                view.FindViewById<TextView>(Resource.Id.rating)
                    .SetBinding((s, v) => s.Text = "" + v, () => viewmodel.Rating);

                view.FindViewById<WebImageView>(Resource.Id.avatar)
                    .SetBinding((s, v) => s.ImageSource = v, () => viewmodel.Avatar);

                view.FindViewById<RatingBar>(Resource.Id.stars)
                    .SetBinding((s, v) => s.Rating = v, () => viewmodel.Stars);

                view.FindViewById<ProgressBar>(Resource.Id.nextStarProgress)
                    .SetBinding((s, v) => s.Progress = (int)(100 * v), () => viewmodel.NextStarProgress);

                Bindings.EndScope();
                return view;
            }
        }

        public class LoginFragment : BaseFragment
        {
            LoginViewModel viewmodel;

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                RetainInstance = true;
                viewmodel = new LoginViewModel();

                MessengerInstance.Register<LoginViewModel.LoginFailMessage>(
                    this, _ => Toast.MakeText(Activity, Resource.String.login_error, ToastLength.Long).Show());
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = inflater.Inflate(Resource.Layout.fragment_login, null);

                var username = view.FindViewById<EditText>(Resource.Id.username);
                Bindings.Add(viewmodel, () => viewmodel.Username, username);

                var password = view.FindViewById<EditText>(Resource.Id.password);
                Bindings.Add(viewmodel, () => viewmodel.Password, password);

                Bindings.BeginScope(viewmodel);

                view.FindViewById(Resource.Id.progress)
                    .SetBinding((s, v) => s.Visibility = v.ToViewStates(), () => viewmodel.IsBusy);

                view.FindViewById(Resource.Id.login).SetCommand(viewmodel.LoginCommand);

                Bindings.EndScope();
                return view;
            }
        }
    }
}