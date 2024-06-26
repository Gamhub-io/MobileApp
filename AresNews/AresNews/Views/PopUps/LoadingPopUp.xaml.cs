﻿using Rg.Plugins.Popup.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views.PopUps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopUp : PopupPage
    {
        public LoadingPopUp(string message = null)
        {
            InitializeComponent();
            if (message != null)
            {
                lblLoading.Text = message;
                // Adapt the siz of the label
                const double oldSize = 18;
                double newSize = message.Length * -0.25 / 18;
                lblLoading.FontSize = newSize > oldSize ? oldSize : newSize;
            }
        }
    }
}