﻿using Newtonsoft.Json;
using System.Reflection;

namespace GamHubApp.Core;

public static class EnvironementSetup
{
    public static void DebugSetup()
    {
        string jsonString;
        string jsonFileName = "AppSettings.json";
        var assembly = Assembly.GetExecutingAssembly();
        Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Config.{jsonFileName}");
        using (var reader = new System.IO.StreamReader(stream))
        {
            jsonString = reader.ReadToEnd();
        }

        foreach (var item in JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonString))
        {
            Environment.SetEnvironmentVariable(item.Key, item.Value);
        }
    }
}
