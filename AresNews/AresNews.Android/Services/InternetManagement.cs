using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AresNews.Droid.Services;
using AresNews.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
[assembly: Xamarin.Forms.Dependency(typeof(InternetManagement))]
namespace AresNews.Droid.Services
{

    public class InternetManagement : IInternetManagement
    {
        [Obsolete]
        public bool TurnWifi(bool status)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.Q)
            {
                WifiManager wifi = (WifiManager)Android.App.Application.Context.GetSystemService(/*Android.App.Application.Context*/Context.WifiService);
                wifi.SetWifiEnabled(status);
                return true;
            }
            return false;
        }
    }
}