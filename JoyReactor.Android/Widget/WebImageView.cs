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

        public void SetImageSource(string imageSource, int fitWidth = 0)
        {
            if (this.imageSource != imageSource)
            {
                this.imageSource = imageSource;
                new ImageRequest()
                    .SetUri(imageSource)
                    .CropIn(GetImageSize(fitWidth).Width, GetImageSize(fitWidth).Height)
                    .To(this);
            }
        }

        Size GetImageSize(int fitWidth)
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

            throw new NotImplementedException("Can't compute thumbnail size");
        }
    }
}