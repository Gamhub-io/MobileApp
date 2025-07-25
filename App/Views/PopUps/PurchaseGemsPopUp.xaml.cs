using CommunityToolkit.Maui.Views;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class PurchaseGemsPopUp : Popup
{
	public PurchaseGemsPopUp(GemTopUpViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
    }


    private void Close_Clicked (object sender, EventArgs e)
    {
		this.CloseAsync().GetAwaiter();
    }
}