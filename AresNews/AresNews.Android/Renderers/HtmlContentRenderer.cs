
using Android.Content;
using AresNews.Controls;
using AresNews.Droid.Renderers;
using LabelHtml.Forms.Plugin.Droid;
using System.ComponentModel;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(HtmlContent), typeof(HtmlContentRenderer))]
namespace AresNews.Droid.Renderers
{
    
    public class HtmlContentRenderer : HtmlLabelRenderer
    {
        public HtmlContentRenderer(Context ctx) : base(ctx)
        {
        }
        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            try
            {
                base.OnElementPropertyChanged(sender, e);
            }
            catch (System.ObjectDisposedException)
            {
                //This is addressing a crash in Xamarin forums 3.6 
            }
        }
    }
}