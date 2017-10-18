using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ETH_Bot.Services
{
    public static class ConfigService
    {
        private static JsonSerializer JsonSerializer = new JsonSerializer();
        private static ConcurrentDictionary<string, string> _configDict = new ConcurrentDictionary<string, string>();

        private static readonly string _filename = "config.json";
        private static bool _initialized = false;
        private static bool _loaded = false;
        
        public static void InitializeLoader(bool force = false)
        {
            if(_initialized && !force)
                return;
            
            JsonSerializer.Converters.Add(new JavaScriptDateTimeConverter());
            JsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            _initialized = true;
        }

        public static void LoadConfig(bool force = false)
        {
            
            if(_loaded && !force)
                return;
            
            if (!File.Exists(_filename))
            {
                throw new IOException($"COULDN'T FIND AND LOAD {Directory.GetCurrentDirectory()}/{_filename}!");
            }
            
            using (StreamReader sr = File.OpenText(_filename))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                _configDict = JsonSerializer.Deserialize<ConcurrentDictionary<string, string>>(reader);
            }
            _loaded = true;
        }

        public static void ConstructLazy(bool force = false)
        {
            InitializeLoader(force);
            LoadConfig(force);
        }

        public static string GetConfigData(string key, bool throwOnError = false)
        {
            string result = "";
            
            var found = _configDict.TryGetValue(key, out result);
            if (!found && throwOnError)
            {
                throw new Exception($"Key {key} not found in {Directory.GetCurrentDirectory()}/{_filename}");
            }
            return result;
        }

        public static ConcurrentDictionary<string, string> GetConfig()
        {
            return _configDict;
        }

        public static string LazyGet(string key, bool throwOnError = false, bool force = false)
        {
            ConstructLazy(force);
            return GetConfigData(key, throwOnError);
        }
    }
}