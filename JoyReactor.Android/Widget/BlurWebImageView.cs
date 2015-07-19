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
                var bufferImage = ThumbnailUtils.ExtractThumbnail(((BitmapDrawable)drawable).Bitmap, 64, 64);

                RenderScript rs = RenderScript.Create(Context);
                var input = Allocation.CreateFromBitmap(rs, bufferImage);
                var output = Allocation.CreateTyped(rs, input.Type);

                var script = ScriptIntrinsicBlur.Create(rs, Element.U8_4(rs));
                script.SetInput(input);
                script.SetRadius(2);
                script.ForEach(output);
                output.CopyTo(bufferImage);

                base.SetImageDrawable(new BitmapDrawable(bufferImage));
            }
        }

        bool CanUseRenderscript()
        {
            return Build.VERSION.SdkInt >= BuildVersionCodes.JellyBeanMr1;
        }
    }
}