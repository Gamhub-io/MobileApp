using AresNews.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            BindingContext = new AppShellViewModel(this);
        }
    }
}