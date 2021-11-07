using AresNews;
using AresNews.iOS.Renderers;
using Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Shell), typeof(ActionableTabRenderer))]
namespace AresNews.iOS.Renderers
{
    public class ActionableTabRenderer : ShellRenderer
    {
        private UIKit.UITabBarItem _prevItem;

        public ActionableTabRenderer() : base()
        {
        }


        //public override void ViewDidAppear(bool animated)
        //{
        //    base.ViewDidAppear(animated);

        //    if (SelectedIndex < TabBar.Items.Length)
        //        _prevItem = TabBar.Items[SelectedIndex];
        //}
        protected override IShellSectionRenderer CreateShellSectionRenderer(ShellSection shellSection)
        {

            return base.CreateShellSectionRenderer(shellSection);
        }
        protected override IShellItemRenderer CreateShellItemRenderer(ShellItem shellItem)
        {
            var item = base.CreateShellItemRenderer(shellItem);
            var f = new ActionableTabItemRenderer(this);
            return item;
        }

        protected override IShellItemTransition CreateShellItemTransition()
        {
            return base.CreateShellItemTransition();
        }
        protected override void OnCurrentItemChanged()
        {
            base.OnCurrentItemChanged();
        }

        //protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
        //{
        //    return new BottomTabBarAppearance();
        //}

        //public override void OnCurrentItemChanged (UIKit.UITabBar tabbar,
        //                                  UIKit.UITabBarItem item)
        //{
        //    base.OnCurrentItemChanged();

        //    if (_prevItem == item )
        //    {

        //    }
        //    _prevItem = item;
        //}
    }
}