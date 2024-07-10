using GamHub.Models;
using GamHub.ViewModels;
using GamHub.ViewModels.PopUps;
using Rg.Plugins.Popup.Pages;
using Microsoft.Maui.Controls.Xaml;

namespace GamHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeleteFeedPopUp : PopupPage
    {
        public DeleteFeedPopUp(Feed feed, FeedsViewModel vm)
        {
            InitializeComponent();
            BindingContext = new DeleteFeedPopUpViewModel(this, feed, vm);
        }
    }
}