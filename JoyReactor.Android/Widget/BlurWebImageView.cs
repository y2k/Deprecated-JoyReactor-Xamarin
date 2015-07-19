using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Media;
using Android.OS;
using Android.Renderscripts;
using Android.Util;

namespace JoyReactor.Android.Widget
{
    public class BlurWebImageView : WebImageView
    {
        public BlurWebImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        {
        }

        public override void SetImageDrawable(Drawable drawable)
        {
            if (drawable != null && CanUseRenderscript())
            {
                RenderScript rs = RenderScript.Create(Context);

                var src = ThumbnailUtils.ExtractThumbnail(((BitmapDrawable)drawable).Bitmap, 64, 64);
                Allocation blurInput = Allocation.CreateFromBitmap(rs, src);

                var script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
                script.SetInput(blurInput);
                script.SetRadius(2);
                script.ForEach(blurInput);

                base.SetImageDrawable(new BitmapDrawable(src));
            }
            else
            {
                base.SetImageDrawable(drawable);
            }
        }

        bool CanUseRenderscript()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1;
        }
    }
}