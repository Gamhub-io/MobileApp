using System;

namespace AresNews.Core
{
    public static class AppConstant
    {
        public static string DiscordClientId = Environment.GetEnvironmentVariable("discord_client_id");
        public static string ApiHost = Environment.GetEnvironmentVariable("api_host");
    }
}
