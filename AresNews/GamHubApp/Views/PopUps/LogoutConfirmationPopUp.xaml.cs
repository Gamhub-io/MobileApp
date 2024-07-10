using GamHub.Models;
using Rg.Plugins.Popup.Pages;
using Microsoft.Maui.Controls.Xaml;

namespace GamHub.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogoutConfirmationPopUp : PopupPage
    {
        public User Profile { get; set; }
        public App CurrentApp { get; }
        public bool? Result { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"> User to logout</param>
        public LogoutConfirmationPopUp(User user)
		{
			InitializeComponent ();

            Profile = user;
			BindingContext = this;
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // prevent the page from collapsing when the keyboard appears
            HasSystemPadding = false;


        }

        private void Cancel_Clicked(object sender, System.EventArgs e)
        {
            Result = false;
        }

        private void Confirm_Clicked(object sender, System.EventArgs e)
        {
            Result = true;
        }
    }
}