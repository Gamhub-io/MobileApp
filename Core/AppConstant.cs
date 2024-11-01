using System;

namespace GamHubApp.Core
{
    public static class AppConstant
    {
        public static string DiscordClientId = Environment.GetEnvironmentVariable("discord_client_id");
        public static string ApiHost = Environment.GetEnvironmentVariable("api_host");
        public static string MonitoringKey = Environment.GetEnvironmentVariable("monitoring_key");
        public static string Localhost = Environment.GetEnvironmentVariable("localhost");
    }
}
