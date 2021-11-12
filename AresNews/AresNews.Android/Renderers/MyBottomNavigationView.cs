using Android.Content;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Google.Android.Material.BottomNavigation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AresNews.Droid.Renderers
{
    internal class MyBottomNavigationView : IShellBottomNavViewAppearanceTracker
    {
        private Context _context;
        private IShellAppearanceElement _shellAppearance;

        public MyBottomNavigationView(Context context)
        {
            this._context = context;
        }

        public void Dispose()
        {
        }

        public void ResetAppearance(BottomNavigationView bottomView)
        {

            IMenu menu = bottomView.Menu;
            for (int i = 0; i < bottomView.Menu.Size(); i++)
            {
                IMenuItem menuItem = menu.GetItem(i);
                var title = menuItem.TitleFormatted;
                Typeface typeface = Typeface.CreateFromAsset(_context.Assets, "Ubuntu-Regular.ttf");
                SpannableStringBuilder sb = new SpannableStringBuilder(title);

               
                sb.SetSpan(new CustomTypefaceSpan("", typeface), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                //sb.SetSpan(new ForegroundColorSpan(_shellAppearance.EffectiveTabBarForegroundColor.ToAndroid()), 0, sb.Length(), SpanTypes.InclusiveInclusive);

                menuItem.SetTitle(sb);
            }

        }

        public void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            _shellAppearance = appearance;
            ResetAppearance(bottomView);
            bottomView.SetBackgroundColor(appearance.EffectiveTabBarBackgroundColor.ToAndroid());
        }
    }
}