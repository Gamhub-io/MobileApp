using AresNews.Models;
using AresNews.ViewModels;
using AresNews.ViewModels.PopUps;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
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