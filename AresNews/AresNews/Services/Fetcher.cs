using AresNews.Models;
using CustardApi.Objects;
using MvvmHelpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AresNews.Services
{
    public class Fetcher
    {
        public static string ProdHost { get; } = "api.gamhub.io";
        public static string LocalHost { get; } = "gamhubdev.ddns.net";
        private static string _dateFormat = "dd-MM-yyy_HH:mm:ss";

        public Service WebService { get; private set; }
        public Fetcher()
        {
#if __LOCAL__
            // Set webservice
            WebService = new Service(host: "gamhubdev.ddns.net",
                                    port: 255,
                                   sslCertificate: false);
#else
            // Set webservice
            WebService = new Service(host: ProdHost,
                                   sslCertificate: true);
#endif
        }
        /// <summary>
        /// Get the last 2 months worth of feed
        /// </summary>
        /// <returns>last 2 months worth of feed</returns>
        public async Task<Collection<Article>> GetMainFeedUpdate()
        {
            return new((await App.WService.Get<Collection<Article>>(controller: "feeds", 
                                                                    action: "update", 
                                                                    parameters: new string[] { DateTime.Now.AddMonths(-2).ToString(_dateFormat) },
                                                                    unSuccessCallback: e => HandleHttpException(e)))
                                                                   .Where(article => article.Blocked == null || article.Blocked == false).ToList());
        }
        /// <summary>
        /// Get the lastest articles since given date
        /// </summary>
        /// <param name="dateUpdate">given date as "dd-MM-yyy_HH:mm:ss"</param>
        /// <returns>lastest articles the date provided</returns>
        public async Task<Collection<Article>> GetMainFeedUpdate(string dateUpdate)
        {
            return new ((await App.WService.Get<Collection<Article>>(controller: "feeds", 
                                                                   action: "update", 
                                                                   parameters: new string[] { dateUpdate },
                                                                   unSuccessCallback: e => HandleHttpException(e)))
                                                                   .Where(article => article.Blocked == null || article.Blocked == false).ToList());
        }
        /// <summary>
        /// Get all the articles of the main feed
        /// </summary>
        /// <returns>the articles</returns>
        public async Task<Collection<Article>> GetMainFeed()
        {
            return new ((await App.WService.Get<Collection<Article>>(controller: "feeds",
                                                                     action: null,
                                                                     unSuccessCallback: e => HandleHttpException(e)))
                                                                     .Where(article => article.Blocked == null || article.Blocked == false).ToList());
        }
        /// <summary>
        /// Get article from a search
        /// </summary>
        /// <param name="text">input of the search</param>
        /// <param name="isUpdate">wether or not it's an update of an existing search</param>
        /// <param name="timeParam">time of the last uppdate</param>
        /// <returns></returns>
        public async Task<ObservableRangeCollection<Article>> GetSearchValues(string text, bool isUpdate, string timeParam = "")
        {
            if (string.IsNullOrEmpty(timeParam))
                isUpdate = false;

            return new ObservableRangeCollection<Article>(( await App.WService.Get<Collection<Article>>(controller: "feeds",
                                                                                                        action: isUpdate ? "update" : null,
                                                                                                        parameters: isUpdate ? new string[] { timeParam } : null,
                                                                                                        jsonBody: $"{{\"search\": \"{text}\"}}",
                                                                                                        unSuccessCallback: e => HandleHttpException(e))));
        }

        private async void HandleHttpException(HttpResponseMessage err)
        {
#if DEBUG
            throw new Exception(await err.Content.ReadAsStringAsync());
#endif
        }
    }
}
