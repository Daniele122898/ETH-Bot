using System.Collections.Generic;

namespace ETH_Bot.Data.Entities.SubEntities
{
    public class Semester
    {
        public List<Class> Classes { get; set; }
        public string Name { get; set; }
    }

    public class @Class
    {
        public string Name { get; set; }
        public string[] Alias { get; set; }
        public string Url { get; set; }
        public string Xpath { get; set; }
        public string Exercise { get; set; }
        public string Solution { get; set; }
        public bool HasExercise { get; set; }
        public bool HasSolution { get; set; }
    }
}