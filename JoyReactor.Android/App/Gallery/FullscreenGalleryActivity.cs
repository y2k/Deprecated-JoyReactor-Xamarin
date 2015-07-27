using Android.App;
using Android.Media;
using Android.OS;
using Android.Views;
using Android.Widget;
using JoyReactor.Android.App.Base;
using JoyReactor.Android.Widget;
using JoyReactor.Core.ViewModels;

namespace JoyReactor.Android.App.Gallery
{
    [Activity(
        Label = "FullscreenGalleryActivity",
        Theme = "@style/AppTheme.Gallery")]
    public class FullscreenGalleryActivity : BaseActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.activity_container);
            if (savedInstanceState == null)
                SupportFragmentManager
                    .BeginTransaction()
                    .Add(Resource.Id.container, new FullscreenGalleryFragment())
                    .Commit();
        }

        public class FullscreenGalleryFragment : BaseFragment
        {
            GalleryViewModel viewmodel;
            VideoView videoView;
            LargeImageViewer imageView;

            public override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                RetainInstance = true;
                viewmodel = Scope.New<GalleryViewModel>();
            }

            public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
            {
                var view = inflater.Inflate(Resource.Layout.activity_fullscreen_gallery, container, false);

                imageView = view.FindViewById<LargeImageViewer>(Resource.Id.imageViewer);
                videoView = view.FindViewById<VideoView>(Resource.Id.videoView);
                videoView.SetOnPreparedListener(new PreparedListenerImlp());

                Bindings.BeginScope(viewmodel);

                view.FindViewById<ProgressBar>(Resource.Id.progress)
                    .SetBinding(UpdateProgress, () => viewmodel.Progress);
                this.SetBinding(UpdateMedia, () => viewmodel.ImagePath);

                Bindings.EndScope();
                return view;
            }

            void UpdateProgress(ProgressBar progress, int value)
            {
                progress.Indeterminate = value == 0;
                progress.Progress = value;
                progress.Visibility = value < 100 ? ViewStates.Visible : ViewStates.Gone;
            }

            void UpdateMedia(FullscreenGalleryFragment _, string value)
            {
                if (value == null)
                    return;
                if (viewmodel.IsVideo)
                {
                    videoView.SetVideoPath(value);
                    videoView.Start();
                }
                else
                {
                    videoView.Visibility = ViewStates.Gone;
                    imageView.SetImage(value);
                }
            }

            class PreparedListenerImlp : Java.Lang.Object, MediaPlayer.IOnPreparedListener
            {
                public void OnPrepared(MediaPlayer mp)
                {
                    mp.Looping = true;
                }
            }
        }
    }
}