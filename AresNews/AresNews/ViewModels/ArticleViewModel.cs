using AresNews.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class ArticleViewModel : BaseViewModel
    {
        private Article _selectedArticle;

        public Article SelectedArticle
        {
            get { return _selectedArticle; }
            set 
            { 
                _selectedArticle = value;
                OnPropertyChanged();
            }
        }
        public Command Browse 
        { 
            get 
            {
                return new Command(async () => await Browser.OpenAsync(_selectedArticle.Url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.External,
                    TitleMode = BrowserTitleMode.Default,
                }));
            }
        }

        public ArticleViewModel(Article article)
        {
            _selectedArticle = article;
        }
    }
}
