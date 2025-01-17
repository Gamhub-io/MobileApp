using GamHubApp.ViewModels;

namespace GamHubApp.Views;

    public partial class BookmarkPage : ContentPage
    {
        public BookmarkPage()
        {
            InitializeComponent();

            BindingContext = new BookmarkViewModel();
        }
    }