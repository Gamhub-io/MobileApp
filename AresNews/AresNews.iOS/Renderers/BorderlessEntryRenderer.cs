
using AresNews.Controls;
using AresNews.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer (typeof(BorderlessEntry), typeof(BorderlessEntryRenderer))]
namespace AresNews.iOS.Renderers
{
    public class BorderlessEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                // Remove boders
                Control.BorderStyle = UITextBorderStyle.None;
            }
        }
    }
}