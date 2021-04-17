
using Android.Content;
using AresNews;
using AresNews.Controls;
using AresNews.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Shell), typeof(ActionableTabRenderer))]

namespace AresNews.Droid.Renderers
{
    public class ActionableTabRenderer : ShellRenderer
    {
        public ActionableTabRenderer(Context context) : base(context)
        {
        }
        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            return new ActionableTabItemRenderer(this);
        }

    }
    
}