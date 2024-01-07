using AresNews.Models;
using CustardApi.Objects;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace AresNews.Services
{
    public class Fetcher
    {
        public static string ProdHost { get; } = "api.gamhub.io";
        public static string LocalHost { get; } = "gamhubdev.ddns.net";

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
        public async Task<Collection<Article>> GetMainFeedUpdate(string lastUpdate)
        {
            return await App.WService.Get<Collection<Article>>(controller: "feeds", action: "update", parameters: new string[] { DateTime.Now.AddMonths(-2).ToString(lastUpdate) }, jsonBody: null);
        }
    }
}
