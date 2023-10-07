using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AresNews.Helpers.Tools
{
    public class KeyboardToolkit
    {
        public void HideKeyBoard()
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }
    }
}
