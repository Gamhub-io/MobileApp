using CommunityToolkit.Maui.Views;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class DealFilterPopUp : Popup
{
	public DealFilterPopUp(DealsViewModel vm)
	{
		InitializeComponent();

		BindingContext = vm;
	}
}