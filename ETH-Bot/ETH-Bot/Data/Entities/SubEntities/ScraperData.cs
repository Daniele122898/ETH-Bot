using System.Collections.Generic;
using HtmlAgilityPack;

namespace ETH_Bot.Data.Entities.SubEntities
{
    public class ScraperData
    {
        public List<HtmlNode> Exercises { get; set; }
        public List<HtmlNode> Solutions { get; set; }
    }
}