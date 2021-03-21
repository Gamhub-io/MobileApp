using AresNews.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePreview : ContentView
    {

        public string ArticleId
        {
            get
            {
                return (string)GetValue(ArticleIdProperty);
            }

            set
            {
                SetValue(ArticleIdProperty, value);
            }
        }
        public static readonly BindableProperty ArticleIdProperty = BindableProperty.Create(
                                                         propertyName: "ArticleId",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultValue: "",
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ArticleTitlePropertyChanged*/);
        public string Headline
        {
            get
            {
                return (string)GetValue(HeadlineProperty);
            }

            set
            {
                SetValue(HeadlineProperty, value);
            }
        }
        public static readonly BindableProperty HeadlineProperty = BindableProperty.Create(
                                                         propertyName: "Headline",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultValue: "",
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: HeadlinePropertyChanged*/);

        public bool ArticleIsSaved
        {
            get
            {
                return (bool)GetValue(ArticleIsSavedProperty);
            }

            set
            {
                SetValue(ArticleIsSavedProperty, value);
            }
        }
        public static readonly BindableProperty ArticleIsSavedProperty = BindableProperty.Create(
                                                         propertyName: "ArticleIsSaved",
                                                         returnType: typeof(bool),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultValue: false,
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ArticleIsSavedPropertyChanged*/);
        public string ArticleTime
        {
            get
            {
                return (string)GetValue(ArticleTimeProperty);
            }

            set
            {
                SetValue(ArticleTimeProperty, value);
            }
        }
        public static readonly BindableProperty ArticleTimeProperty = BindableProperty.Create(
                                                         propertyName: "ArticleTime",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ArticleTimePropertyChanged*/);

        public string ArticleSource
        {
            get
            {
                return (string)GetValue(ArticleSourceProperty);
            }

            set
            {
                SetValue(ArticleSourceProperty, value);
            }
        }
        public static readonly BindableProperty ArticleSourceProperty = BindableProperty.Create(
                                                         propertyName: "ArticleSource",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultValue: "",
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ArticleSourcePropertyChanged*/);
        public string Thumnail
        {
            get
            {
                return (string)GetValue(ThumnailProperty);
            }

            set
            {
                SetValue(ThumnailProperty, value);
            }
        }
        public static readonly BindableProperty ThumnailProperty = BindableProperty.Create(
                                                         propertyName: "Thumnail",
                                                         returnType: typeof(string),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultValue: "",
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ThumnailPropertyChanged*/);
        public Command BookmarkCommand
        {
            get
            {
                return (Command)GetValue(BookmarkCommandProperty);
            }

            set
            {
                SetValue(BookmarkCommandProperty, value);
            }
        }
            

        public static readonly BindableProperty BookmarkCommandProperty = BindableProperty.Create(
                                                                 propertyName: "BookmarkCommand",
                                                                 returnType: typeof(Command),
                                                                 declaringType: typeof(ArticlePreview),
                                                                 defaultBindingMode: BindingMode.TwoWay
                                                                 /*propertyChanged: BookmarkCommandChanged*/);

        public Command ShareCommand 
        {
            get
            {
                return (Command)GetValue(ShareCommandProperty);
            }

            set
            {
                SetValue(ShareCommandProperty, value);
            }
        }

        public static readonly BindableProperty ShareCommandProperty = BindableProperty.Create(
                                                         propertyName: "ShareCommand",
                                                         returnType: typeof(Command),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ShareCommandChanged*/);
        public object ShareCommandParameter
        {
            get
            {
                return (object)GetValue(ShareCommandParameterProperty);
            }

            set
            {
                SetValue(ShareCommandParameterProperty, value);
            }
        }

        public static readonly BindableProperty ShareCommandParameterProperty = BindableProperty.Create(
                                                         propertyName: "ShareCommandParameter",
                                                         returnType: typeof(object),
                                                         declaringType: typeof(ArticlePreview),
                                                         defaultBindingMode: BindingMode.TwoWay
                                                         /*propertyChanged: ShareCommandChanged*/);


        





        public ArticlePreview()
        {
            InitializeComponent();
        }
    }
}