using AresNews.Models;
using CustardApi.Objects;
using System;
using System.Collections.ObjectModel;
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
            try
            {

                return await App.WService.Get<Collection<Article>>(controller: "feeds",
                                                                   action: "update",
                                                                   parameters: new string[] { DateTime.Now.AddMonths(-2).ToString(_dateFormat) },
                                                                   jsonBody: null,
                                                                   unSuccessCallback: e => HandleHttpException(e));
            }
            catch (Exception ex)
            {

#if DEBUG
                throw ex;
#else
                return null;
#endif
            }
        }
        /// <summary>
        /// Get the lastest articles since given date
        /// </summary>
        /// <param name="dateUpdate">given date as "dd-MM-yyy_HH:mm:ss"</param>
        /// <returns>lastest articles the date provided</returns>
        public async Task<Collection<Article>> GetMainFeedUpdate(string dateUpdate)
        {
            try
            {

                return await App.WService.Get<Collection<Article>>(controller: "feeds",
                                                                   action: "update",
                                                                   parameters: new string[] { dateUpdate },
                                                                   jsonBody: null,
                                                                   unSuccessCallback: e => HandleHttpException(e));
            }
            catch (Exception ex)
            {

#if DEBUG
                throw ex;
#else
                return null; 
#endif
            }
        }

        private async void HandleHttpException(HttpResponseMessage err)
        {
#if DEBUG
            throw new Exception(await err.Content.ReadAsStringAsync());
#endif
        }
    }
}
