
using Android.OS;
using Android.Views;
using AndroidViews = Android.Views;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Platform.Compatibility;
using Microsoft.Maui.Controls.PlatformConfiguration.AndroidSpecific;

namespace GamHubApp.Platforms.Android.Renderers;

public class ActionableTabItemRenderer : ShellItemRenderer
{
    BottomNavigationView _bottomView;
    public ActionableTabItemRenderer(IShellContext shellContext) : base(shellContext)
    {
    }

    /// <summary>
    /// When we click on a current tab
    /// </summary>
    /// <param name="shellSection"></param>
    protected override void OnTabReselected(ShellSection shellSection)
    {

        // Send an action to go on top of the current feed
        (ShellItem as IShellItemController).ProposeSection(ShellItem.CurrentItem);
    }
    public override AndroidViews.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
    {
        AndroidViews.View outerlayout = base.OnCreateView(inflater, container, savedInstanceState);

        // Get the bottom view
        _bottomView = outerlayout.FindViewById<BottomNavigationView>(2130772341);

        // Remove the title if it's null
        //_bottomView.LabelVisibilityMode = LabelVisibilityMode.LabelVisibilityUnlabeled;



        return outerlayout;
    }
}