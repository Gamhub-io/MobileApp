using CommunityToolkit.Maui.Views;
using GamHubApp.Models;

namespace GamHubApp.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LogoutConfirmationPopUp : Popup
    {
        public User Profile { get; set; }
        public App CurrentApp { get; }
        public bool? ResponseResult { get; set; } = null;

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

        private void Cancel_Clicked(object sender, System.EventArgs e)
        {
            ResponseResult = false;
        }

        private void Confirm_Clicked(object sender, System.EventArgs e)
        {
            ResponseResult = true;
        }
    }
}