using System;

namespace GamHubApp.Core
{
    public static class AppConstant
    {
        public static string DiscordClientId = Environment.GetEnvironmentVariable("discord_client_id");
        public static string ApiHost = Environment.GetEnvironmentVariable("api_host");
        public static string MonitoringKey = Environment.GetEnvironmentVariable("monitoring_key");
        public static string Localhost = Environment.GetEnvironmentVariable("localhost");

        public const string DbFilename = "ares.db3";
        public const string DbBackUpFilename = "aresBackup.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string GeneralDBpath =>
            Path.Combine(FileSystem.AppDataDirectory, DbFilename);

        public static string PathDBBackUp =>
            Path.Combine(FileSystem.AppDataDirectory, DbBackUpFilename);

        // Settings - preferences
        public const string DealArticleEnable = "DEAL_ARTICLE_ENABLE";
        public const string DealPageEnable = "DEAL_PAGE_ENABLE";
    }
}
