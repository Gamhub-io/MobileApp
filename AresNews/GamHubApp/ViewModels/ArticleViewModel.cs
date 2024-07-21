using GamHub.Models;
using SQLiteNetExtensions.Extensions;
using System.Diagnostics;

namespace GamHub.ViewModels
{
    public class ArticleViewModel : BaseViewModel
    {
        private CancellationTokenSource _cts;
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
        private bool _isMenuOpen;

        public bool IsMenuOpen
        {
            get { return _isMenuOpen; }
            set 
            {
                _isMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
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
        private string _ttsColour;

        public string TtsColour
        {
            get 
            { 
                return _ttsColour; 
            }
            set 
            {
                _ttsColour = value; 
                OnPropertyChanged(nameof(TtsColour));
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
                        // Stop the tts if already launched
                        if (_audioIsPlaying)
                        {
                            StopTtS();
                            return;
                        }

                        _cts = new CancellationTokenSource();

                        // indicator text to speech done
                        bool ttsDone = false;

                        // Run text to speech
                        await Task.Factory.StartNew(async () =>
                        {
                            await TextToSpeech.SpeakAsync(SelectedArticle.TextSnipet, _cts.Token);
                            ttsDone = true;
                        });
                        AudioIsPlaying = !_audioIsPlaying;

                        // Change the icon
                        if (_audioIsPlaying)
                            await Task.Run(() =>
                            {
                                TtsColour = "#222326";
                                while (_audioIsPlaying && !ttsDone)
                                {
                                    //TtsIcon = "\uf6a8";
                                    //Thread.Sleep(500);
                                    TtsIcon = "\uf028";
                                    Thread.Sleep(500);
                                    TtsIcon = "\uf027";
                                    Thread.Sleep(500);
                                }
                                StopTtS();
                            });
                    } 
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                });
            }
        }
        /// <summary>
        /// Stop text to speach
        /// </summary>
        public void StopTtS()
        {
            CancelSpeech();
            // Reset icon
            TtsIcon = "\uf028";
            TtsColour = "#36383c";

            // Mark the audio as not playin
            AudioIsPlaying = false;
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
            _ttsColour = "#36383c";

            // Register Hook
            _ =(App.Current as App).DataFetcher.RegisterHook(article);

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
            _shareArticle = new Command(async () =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = _selectedArticle.Url,
                    Title = "Share this article",
                    Subject =_selectedArticle.Title,
                    Text = _selectedArticle.Title
                    
                });
            });

            _selectedArticle = article;

            TimeSpent = new Stopwatch();
            TimeSpent.Start();
        }
        /// <summary>
        /// Cancel text to speech
        /// </summary>
        public void CancelSpeech()
        {
            if (_cts?.IsCancellationRequested ?? true)
                return;

            _cts.Cancel();
        }
    }
}
