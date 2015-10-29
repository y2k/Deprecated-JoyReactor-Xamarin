using System;
using Android.Support.V7.Widget;
using Android.Views.Animations;
using JoyReactor.Android.App.Common;

namespace JoyReactor.Android.Widget
{
    public class ToolbarScrollHider
    {
        readonly RecyclerView list;
        readonly Toolbar toolbar;

        bool blockAnimation;

        public ToolbarScrollHider(Toolbar toolbar, RecyclerView list)
        {
            this.toolbar = toolbar;
            this.list = list;
        }

        public void Attach()
        {
            list.AddOnScrollListener(new OnScrollListenerImpl(OnScrolled));
        }

        async void OnScrolled(int dy)
        {
            if (blockAnimation)
                return;

            if (dy > 0 && toolbar.TranslationY == 0)
            {
                blockAnimation = true;
                await toolbar.Animate().TranslationY(-toolbar.Height).SetInterpolator(new AccelerateInterpolator()).EndAsync();
                blockAnimation = false;
            }
            else if (dy <= 0 && toolbar.TranslationY == -toolbar.Height)
            {
                blockAnimation = true;
                await toolbar.Animate().TranslationY(0).SetInterpolator(new DecelerateInterpolator()).EndAsync();
                blockAnimation = false;
            }
        }

        class OnScrollListenerImpl : RecyclerView.OnScrollListener
        {
            Action<int> onScrolled;

            public OnScrollListenerImpl(Action<int> onScrolled)
            {
                this.onScrolled = onScrolled;
            }

            public override void OnScrolled(RecyclerView recyclerView, int dx, int dy)
            {
                onScrolled(dy);
            }
        }
    }
}