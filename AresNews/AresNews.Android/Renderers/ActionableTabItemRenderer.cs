using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
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
        /// Pops to root when the selected tab is pressed.
        /// </summary>
        /// <param name="shellSection"></param>
        protected override void OnTabReselected(ShellSection shellSection)
        {
            //Xamarin.Forms.Device.BeginInvokeOnMainThread(async () =>
            //{
            //    await shellSection?.Navigation.PopToRootAsync();
            //});
        }
    }
}