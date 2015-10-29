using System;
using Android.Content;
using Android.Util;
using Android.Widget;
using JoyReactor.Android.Model;

namespace JoyReactor.Android.Widget
{
    public class WebImageView : ImageView
    {
        string imageSource;

        public WebImageView(Context context, IAttributeSet attrs)
            : base(context, attrs)
        { 
        }

        public void SetImageSource(string imageSource, int fitWidth = 0, float imageAspect = 0)
        {
            if (this.imageSource != imageSource)
            {
                this.imageSource = imageSource;
                var size = GetImageSize(fitWidth, imageAspect);
                new ImageRequest()
                    .SetUri(imageSource)
                    .CropIn(size.Width, size.Height)
                    .To(this);
            }
        }

        Size GetImageSize(int fitWidth, float imageAspect)
        {
            if (LayoutParameters.Width > 0 && LayoutParameters.Height > 0)
                return new Size(LayoutParameters.Width, LayoutParameters.Height);
            
            var aspectPanel = Parent as FixedAspectPanel;
            if (aspectPanel != null)
            {
                if (LayoutParameters.Width > 0)
                    return new Size(LayoutParameters.Width, (int)(LayoutParameters.Width / aspectPanel.Aspect));
                if (LayoutParameters.Height > 0)
                    return new Size((int)(LayoutParameters.Height * aspectPanel.Aspect), LayoutParameters.Height);
                if (fitWidth > 0)
                    return new Size(fitWidth, (int)(fitWidth / aspectPanel.Aspect));
            }

            if (imageAspect > 0 && fitWidth > 0)
                return new Size(fitWidth, (int)(fitWidth / imageAspect));

            throw new NotImplementedException("Can't compute thumbnail size");
        }
    }
}