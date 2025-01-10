using Android.Content.Res;
using Android.Graphics;
using Android.Text;
using Android.Views;
using AndroidResource = Android.Resource;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Platform;

namespace GamHubApp.Platforms.Android.Renderers;

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

            menuItem.SetTitle(sb);
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

        bottomView.SetBackgroundColor(_shellAppearance.EffectiveTabBarBackgroundColor.ToPlatform());

        int[][] states = new int[][]
        {
               // disabled
               new int[] { -AndroidResource.Attribute.StateChecked },  // pressed

               new int[] { AndroidResource.Attribute.StateChecked}, // unchecked
        };

        int[] colours = new int[]
        {
            _shellAppearance.EffectiveTabBarUnselectedColor.ToPlatform(),
            _shellAppearance.EffectiveTabBarTitleColor.ToPlatform(),
        };

        ColorStateList colorStateList = new ColorStateList(states, colours);

        bottomView.ItemIconTintList = colorStateList;
        bottomView.ItemTextColor = colorStateList;
    }
}
