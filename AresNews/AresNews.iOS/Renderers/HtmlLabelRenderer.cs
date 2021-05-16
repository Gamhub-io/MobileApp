using AresNews.Controls;
using AresNews.iOS.Renderers;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(HtmlContent), typeof(HtmlContentRenderer))]
namespace AresNews.iOS.Renderers
{
    public class HtmlContentRenderer : LabelRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
           

        }
    }
}