using System;
using System.Collections.Generic;
using ETH_Bot.Data.Entities.SubEntities;
using HtmlAgilityPack;

namespace ETH_Bot.Services
{
    public static class ScraperService
    {
        public static ScraperData Scrape(string url, string xpath, string ex, string sol, bool hasEx, bool hasSol)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes(xpath);

            if (nodes == null || nodes.Count == 0) return null;
            
            List<HtmlNode> exercises = new List<HtmlNode>();
            List<HtmlNode> solutions = new List<HtmlNode>();

            foreach (var node in nodes)
            {
                if (hasEx)
                {
                    if (node.InnerText.Trim().StartsWith(ex, StringComparison.OrdinalIgnoreCase))
                    {
                        exercises.Add(node);
                        continue;
                    }
                }

                if (hasSol)
                {
                    if (node.InnerText.Trim().StartsWith(sol, StringComparison.OrdinalIgnoreCase))
                    {
                        solutions.Add(node);
                    }
                }
            }
            
            return new ScraperData()
            {
                Exercises = exercises,
                Solutions = solutions
            };
            
        }
    }
}