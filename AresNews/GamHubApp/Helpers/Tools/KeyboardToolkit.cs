using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHub.Helpers.Tools
{
    public class KeyboardToolkit
    {
        public void HideKeyBoard()
        {
            DependencyService.Get<IKeyboardHelper>().HideKeyboard();
        }
    }
}
