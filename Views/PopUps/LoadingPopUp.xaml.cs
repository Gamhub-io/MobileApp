using CommunityToolkit.Maui.Views;

namespace GamHubApp.Views.PopUps
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPopUp : Popup
    {
        public LoadingPopUp(string message = null)
        {
            InitializeComponent();
            if (message != null)
            {
                lblLoading.Text = message;
                // Adapt the siz of the label
                const double oldSize = 18;
                double newSize = message.Length * -0.25 / 18;
                lblLoading.FontSize = newSize > oldSize ? oldSize : newSize;
            }
        }
    }
}