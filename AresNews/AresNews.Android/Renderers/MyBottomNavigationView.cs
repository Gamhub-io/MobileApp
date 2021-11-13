using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Text.Style;
using Android.Views;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.Tabs;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AresNews.Droid.Renderers
{
    internal class MyBottomNavigationView : IShellBottomNavViewAppearanceTracker
    {
        private IShellContext _context;
        private IShellAppearanceElement _shellAppearance;

        public MyBottomNavigationView(IShellContext context)
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
                Typeface typeface = Typeface.CreateFromAsset(_context.AndroidContext.Assets, "Ubuntu-Regular.ttf");
                SpannableStringBuilder sb = new SpannableStringBuilder(title);

               
                sb.SetSpan(new CustomTypefaceSpan("", typeface), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                //sb.SetSpan(new ForegroundColorSpan(_shellAppearance.EffectiveTabBarForegroundColor.ToAndroid()), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                
                menuItem.SetTitle(sb);
            }

        }

        public void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
        {
            _shellAppearance = appearance;
            IMenu menu = bottomView.Menu;
            for (int i = 0; i < menu.Size(); i++)
            {
                IMenuItem menuItem = menu.GetItem(i);
                var title = menuItem.TitleFormatted;
                Typeface typeface = Typeface.CreateFromAsset(_context.AndroidContext.Assets, "Ubuntu-Regular.ttf");
                SpannableStringBuilder sb = new SpannableStringBuilder(title);

                sb.SetSpan(new CustomTypefaceSpan("", typeface), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                //sb.SetSpan(new ForegroundColorSpan(_shellAppearance.EffectiveTabBarForegroundColor.ToAndroid()), 0, sb.Length(), SpanTypes.InclusiveInclusive);
                //sb.SetSpan(new ForegroundColorSpan(_shellAppearance.EffectiveTabBarForegroundColor.ToAndroid()), 0, sb.Length(), SpanTypes.InclusiveInclusive);

                menuItem.SetTitle(sb);
                //_context.AndroidContext
            }

            SetBottomViewColours(bottomView);
        }
        /// <summary>
        /// Set the colours of a  bottomView based on the shell config
        /// </summary>
        /// <param name="bottomView">the bottom view</param>
        private void SetBottomViewColours(BottomNavigationView bottomView)
        {
            bottomView.ItemIconTintList = null;
            bottomView.ItemTextColor = null;

            bottomView.SetBackgroundColor(_shellAppearance.EffectiveTabBarBackgroundColor.ToAndroid());

            int[][] states = new int[][]
            {
                   // disabled
                   new int[] { -Android.Resource.Attribute.StateChecked },  // pressed

                   new int[] {Android.Resource.Attribute.StateChecked}, // unchecked
            };
            //string unselectedColorHex = _shellAppearance.EffectiveTabBarUnselectedColor.ToHex();
            int[] colours = new int[]
            {
                _shellAppearance.EffectiveTabBarUnselectedColor.ToAndroid(),
                _shellAppearance.EffectiveTabBarTitleColor.ToAndroid(),
            };

            ColorStateList colorStateList = new ColorStateList(states, colours);
            
            bottomView.ItemIconTintList = colorStateList;
            bottomView.ItemTextColor = colorStateList;
        }
    }
}