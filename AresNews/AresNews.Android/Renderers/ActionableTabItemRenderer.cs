using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace AresNews.Droid.Renderers
{
    public class ActionableTabItemRenderer : ShellItemRenderer
    {
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
            MessagingCenter.Send<MessageItem>(new MessageItem () { Id = Guid.NewGuid() }, "ScrollTop");
        }
    }
}