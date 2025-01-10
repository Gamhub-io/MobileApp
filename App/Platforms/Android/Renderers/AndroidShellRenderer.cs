using Android.Content;
using Google.Android.Material.BottomNavigation;
using Microsoft.Maui.Controls.Handlers.Compatibility;
using Microsoft.Maui.Controls.Platform.Compatibility;

namespace GamHubApp.Platforms.Android.Renderers;

public class AndroidShellRenderer : ShellRenderer
{
    public AndroidShellRenderer(Context context) : base(context)
    {

    }
    protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
    {
        return new ActionableTabItemRenderer(this);
    }

    protected override IShellBottomNavViewAppearanceTracker CreateBottomNavViewAppearanceTracker(ShellItem shellItem)
    {
        if (shellItem.Items.Count == 1)
            return base.CreateBottomNavViewAppearanceTracker(shellItem);
        return new MyBottomNavigationView(this);
    }
}

public class CustomShellBottomNavViewAppearanceTracker : ShellBottomNavViewAppearanceTracker
{
    private readonly IShellContext _shellRenderer;
    private readonly ShellItem _shellItem;
    private bool _subscribedItemReselected;

    public CustomShellBottomNavViewAppearanceTracker(IShellContext shellRenderer, ShellItem shellItem)
        : base(shellRenderer, shellItem)
    {
        _shellRenderer = shellRenderer;
        _shellItem = shellItem;
    }

    public override void SetAppearance(BottomNavigationView bottomView, IShellAppearanceElement appearance)
    {
        if (_subscribedItemReselected) return;
        bottomView.ItemReselected += ItemReselected;
        _subscribedItemReselected = true;
    }

    private void ItemReselected(object? sender, EventArgs e)
    {
        (_shellItem as IShellItemController).ProposeSection(_shellItem.CurrentItem);
    }
}
