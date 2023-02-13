using AresNews.Models;
using AresNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditFeedPage : ContentPage
	{
		public EditFeedPage (Feed feed, FeedsViewModel vm)
		{
			InitializeComponent ();
			BindingContext = new EditFeedViewModel(feed, vm);
		}

        private void Button_Clicked(object sender, EventArgs e)
        {

        }
    }
}