using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookmarkButton : ContentView
    {
        public bool IsSaved
        {
            get
            {
                return (bool)GetValue(IsSavedProperty);
            }

            set
            {
                SetValue(IsSavedProperty, value);
            }
        }
        public static readonly BindableProperty IsSavedProperty = BindableProperty.Create(
                                                         propertyName: "IsSaved",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(BookmarkButton),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }

            set
            {
                SetValue(CommandProperty, value);
            }
        }
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(
                                                         propertyName: "Command",
                                                         returnType: typeof(ICommand),
                                                         declaringType: typeof(BookmarkButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public object CommandParameter
        {
            get
            {
                return (object)GetValue(CommandParameterProperty);
            }

            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(
                                                         propertyName: "CommandParameter",
                                                         returnType: typeof(object),
                                                         declaringType: typeof(BookmarkButton),
                                                         defaultValue: null,
                                                         defaultBindingMode: BindingMode.TwoWay);
        public BookmarkButton()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            IsSaved = !IsSaved;
        }
    }
}