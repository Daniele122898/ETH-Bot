using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using ETH_Bot.Data.Entities.SubEntities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ETH_Bot.Services
{
    public class SemesterService
    {
        public ConcurrentBag<Semester> SemesterData = new ConcurrentBag<Semester>();
        private JsonSerializer _jsonSerializer = new JsonSerializer();

        private const string PATH = "ScrapeData/";

        public void Initialize()
        {
            // initialize JSON serializer
            _jsonSerializer.Converters.Add(new JavaScriptDateTimeConverter());
            _jsonSerializer.NullValueHandling = NullValueHandling.Ignore;
            // load all jsons
            foreach (var file in Directory.GetFiles(PATH))
            {
                using (StreamReader sr = File.OpenText(file))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var data = _jsonSerializer.Deserialize<Semester>(reader);
                    if (data != null)
                    {
                        SemesterData.Add(data);
                    }
                }
            }
        }

        public void ReloadData()
        {
            // load all jsons
            foreach (var file in Directory.GetFiles(PATH))
            {
                using (StreamReader sr = File.OpenText(file))
                using (JsonReader reader = new JsonTextReader(sr))
                {
                    var data = _jsonSerializer.Deserialize<Semester>(reader);
                    if (data != null)
                    {
                        SemesterData.Add(data);
                    }
                }
            }
        }

    }
}