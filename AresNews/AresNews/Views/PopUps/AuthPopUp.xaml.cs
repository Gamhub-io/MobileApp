
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AuthPopUp : PopupPage
    {
		public AuthPopUp()
		{
			InitializeComponent ();
		}
        protected override void OnAppearing()
        {
            base.OnAppearing();

            // prevent the page from collapsing when the keyboard appears
            HasSystemPadding = false;


        }

        private void Discord_Clicked(object sender, System.EventArgs e)
        {

        }
    }
}