using AresNews.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;

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

        public ArticleViewModel(Article article)
        {
            _selectedArticle = article;
        }
    }
}
