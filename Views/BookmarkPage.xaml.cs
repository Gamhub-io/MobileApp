using GamHubApp.ViewModels;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHubApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookmarkPage : ContentPage
    {
        public BookmarkPage()
        {
            InitializeComponent();

            BindingContext = new BookmarkViewModel();
        }
    }
}