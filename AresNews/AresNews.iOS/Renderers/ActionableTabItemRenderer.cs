using AresNews.Models;
using Foundation;
using System;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace AresNews.iOS.Renderers
{
    public class ActionableTabItemRenderer : ShellItemRenderer
    {

        private UIKit.UITabBarItem _prevItem;

        public ActionableTabItemRenderer(IShellContext context) : base(context)
        {
        }


        /// <summary>
        /// When we click on a current tab
        /// </summary>
        /// <param name="shellSection"></param>
        public override void ItemSelected(UIKit.UITabBar tabbar,
                                          UIKit.UITabBarItem item)
        {
            base.ItemSelected(tabbar, item);
            if (_prevItem == item)
            {
                MessagingCenter.Send<MessageItem>(new MessageItem() { Id = Guid.NewGuid() }, "ScrollTop");
            }
            _prevItem = item;
        }
        //public override void Select(NSObject sender)
        //{
        //    base.Select(sender);
        //    // Send an action to go on top of the current feed
        //    MessagingCenter.Send<MessageItem>(new MessageItem() { Id = Guid.NewGuid() }, "ScrollTop");

        //}
    }
}