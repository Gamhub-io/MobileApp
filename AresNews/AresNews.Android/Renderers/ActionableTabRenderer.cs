
using Android.Content;
using Android.Graphics;
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

        protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
        {
            return new MyBottomNavigationView(this.AndroidContext);
            //Typeface.CreateFromAsset(this.AndroidContext.Assets, "Ubuntu-Regular.ttf");
            //.SetTypeface(typeface, TypefaceStyle.Italic);
            //return base.CreateBottomNavViewAppearanceTracker(shellItem);
        }
    }
    
}