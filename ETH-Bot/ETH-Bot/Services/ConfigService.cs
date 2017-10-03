﻿using System.Collections.Concurrent;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ETH_Bot.Services
{
    public static class ConfigService
    {
        private static JsonSerializer JsonSerializer = new JsonSerializer();
        private static ConcurrentDictionary<string, string> _configDict = new ConcurrentDictionary<string, string>();

        public static void InitializeLoader()
        {
            JsonSerializer.Converters.Add(new JavaScriptDateTimeConverter());
            JsonSerializer.NullValueHandling = NullValueHandling.Ignore;
        }

        public static void LoadConfig()
        {
            if (!File.Exists("config.json"))
            {
                throw new IOException("COULDN'T FIND AND LOAD CONFIG FILE!");
            }
            
            using (StreamReader sr = File.OpenText("config.json"))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                _configDict = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(reader);
            }
        }

        public static string GetConfigData(string key)
        {
            string result = "";
            _configDict.TryGetValue(key, out result);
            return result;
        }

        public static ConcurrentDictionary<string, string> GetConfig()
        {
            return _configDict;
        }
    }
}