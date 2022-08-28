using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        public Command MailTo
        {
            get
            {
                return new Command<string>(async (address) => await Email.ComposeAsync(subject: "", body: "", to: new string[] { address }));
            }
        }
    }
}
