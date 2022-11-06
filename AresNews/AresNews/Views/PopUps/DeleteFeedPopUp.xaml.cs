using AresNews.Models;
using AresNews.ViewModels.PopUps;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteFeedPopUp : PopupPage
    {
        public DeleteFeedPopUp(Feed feed)
        {
            InitializeComponent();
            BindingContext = new DeleteFeedPopUpViewModel(this, feed);
        }
    }
}