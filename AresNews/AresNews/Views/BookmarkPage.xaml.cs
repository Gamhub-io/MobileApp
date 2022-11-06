using AresNews.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
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