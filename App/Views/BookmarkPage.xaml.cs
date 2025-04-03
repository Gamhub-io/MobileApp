using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class BookmarkPage : ContentPage
{
    public BookmarkPage(BookmarkViewModel vm)
    {
        InitializeComponent();

        BindingContext = vm;
    }
}