using AresNews.Models;
using MvvmHelpers;
using SQLiteNetExtensions.Extensions;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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
        /// <summary>
        /// Time spend reading the article
        /// </summary>
        public Stopwatch TimeSpent { get; set; }

        // Command to add a Bookmark
        private readonly Command _addBookmark;

        public Command AddBookmark
        {
            get { return _addBookmark; }
        }
        // Command to add a Bookmark
        private readonly Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }
        public Command Browse 
        { 
            get 
            {
                return new Command(async () => await Browser.OpenAsync(_selectedArticle.Url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Default,
                }));
            }
        }
        private string _ttsIcon;

        public string TtsIcon
        {
            get 
            { 
                return _ttsIcon; 
            }
            set 
            {
                _ttsIcon = value; 
                OnPropertyChanged(nameof(TtsIcon));
            }
        }

        public Command PlayTextToSpeech 
        { 
            get 
            {
                return new Command<string>(async (text) =>
                {
                    try
                    {
                        AudioIsPlaying = !_audioIsPlaying;

                        // Change the icon
                        if (_audioIsPlaying)
                            await Task.Run(() =>
                            {
                                
                                while (_audioIsPlaying)
                                {
                                    //TtsIcon = "\uf6a8";
                                    //Thread.Sleep(500);
                                    TtsIcon = "\uf028";
                                    Thread.Sleep(500);
                                    TtsIcon = "\uf027";
                                    Thread.Sleep(500);
                                    //switch (_ttsIcon)
                                    //{
                                    //    case "\uf027":
                                    //        TtsIcon = "\uf6a8";
                                    //        Thread.Sleep(100);
                                    //        break;
                                    //    case "\uf6a8":
                                    //        TtsIcon = "\uf028";
                                    //        Thread.Sleep(100);
                                    //        break;
                                    //    case "\uf028":
                                    //        TtsIcon = "\uf027";
                                    //        Thread.Sleep(100);
                                    //        break;
                                    //    default:
                                    //        break;
                                    //}
                                }
                            });
                    } 
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                });
            }
        }
        private bool  _audioIsPlaying;

        public bool AudioIsPlaying
        {
            get 
            {
                return _audioIsPlaying; 
            }
            set 
            { 
                _audioIsPlaying = value; 
                OnPropertyChanged(nameof(AudioIsPlaying));
            }
        }
        


        public ArticleViewModel(Article article)
        {
            _ttsIcon = "\uf028";

            _addBookmark = new Command((id) =>
            {
                // If the article is already in bookmarks
                bool isSaved = _selectedArticle.IsSaved;

                // Marked the article as saved
                SelectedArticle.IsSaved = !isSaved;


                if (isSaved)
                    App.SqLiteConn.Delete(_selectedArticle);
                else
                    // Insert it in database
                    App.SqLiteConn.InsertWithChildren(_selectedArticle, recursive: true);

                MessagingCenter.Send<Article>(_selectedArticle, "SwitchBookmark");


            });


            // Set command to share an article
            _shareArticle = new Command(async (url) =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = url.ToString(),
                    Title = "Share this article",
                    Subject =_selectedArticle.Title,
                    Text = _selectedArticle.Title
                    
                });
            });

            _selectedArticle = article;

            TimeSpent = new Stopwatch();
            TimeSpent.Start();
        }
    }
}
