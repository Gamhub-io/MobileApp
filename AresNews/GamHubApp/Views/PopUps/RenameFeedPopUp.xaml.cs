﻿using GamHub.Models;
using GamHub.ViewModels;
using GamHub.ViewModels.PopUps;
using Rg.Plugins.Popup.Pages;
using Microsoft.Maui.Controls.Xaml;

namespace GamHub.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class RenameFeedPopUp : PopupPage
    {
		public RenameFeedPopUp (Feed feed, FeedsViewModel vm)
		{
			InitializeComponent ();
			BindingContext = new RenameFeedPopUpViewModel (this,feed, vm);
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // prevent the page from collapsing when the keyboard appears
            HasSystemPadding = false;


        }
    }
}