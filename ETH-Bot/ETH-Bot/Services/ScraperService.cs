using System.Collections.Generic;
using ETH_Bot.Data.Entities.SubEntities;
using HtmlAgilityPack;

namespace ETH_Bot.Services
{
    public static class ScraperService
    {
        public static ScraperData ScrapeLinAlg(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/div[1]/table[1]//tr//td//a");
            
            List<HtmlNode> exercises = new List<HtmlNode>();
            List<HtmlNode> solutions = new List<HtmlNode>();

            foreach (var node in nodes)
            {
                //this is an Solution
                if (node.InnerText.StartsWith("Lösung"))
                {
                    solutions.Add(node);
                }
                else if (node.InnerText.StartsWith("Serie"))
                {
                    exercises.Add(node);
                }
            }
            return new ScraperData()
            {
                Exercises = exercises,
                Solutions = solutions
            };
        }

        public static ScraperData ScrapeAlgDat(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url+"index.html");
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div[3]/div/table[2]//tr//td//a");
            
            List<HtmlNode> exercises = new List<HtmlNode>();
            List<HtmlNode> solutions = new List<HtmlNode>();

            foreach (var node in nodes)
            {
                //this is an Solution
                if (node.InnerText.StartsWith("B"))
                {
                    solutions.Add(node);
                }
                else
                {
                    exercises.Add(node);
                }
            }
            return new ScraperData()
            {
                Exercises = exercises,
                Solutions = solutions
            };
        }

        public static ScraperData ScrapeDiscMath(string url)
        {
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/div[2]/table[1]//tr//td//a"); // double slash what you want to completely search
            
            List<HtmlNode> exercises = new List<HtmlNode>();
            List<HtmlNode> solutions = new List<HtmlNode>();

            foreach (var node in nodes)
            {
                //this is an exercise
                if (node.InnerText.StartsWith("&"))
                {
                    exercises.Add(node);
                }
                else
                {
                    solutions.Add(node);
                }
            }
            return new ScraperData()
            {
                Exercises = exercises,
                Solutions = solutions
            };
        }

        public static ScraperData ScrapeEprog()
        {
            var web = new HtmlWeb();
            var doc = web.Load("http://www.lst.inf.ethz.ch/education/einfuehrung-in-die-programmierung-i--252-0027-.html");
            var nodes = doc.DocumentNode.SelectNodes("/html/body/section/div[2]/section/div/div/div[1]/div/div[5]/div/div/div/div/div/div/div/div/div/table//tr//td//a");
         
            //&Uuml;bung 
            List<HtmlNode> exercises = new List<HtmlNode>();

            foreach (var node in nodes)
            {
                //this is an exercise
                if (node.InnerText.StartsWith("&Uuml;bung "))
                {
                    exercises.Add(node);
                }
            }
            return new ScraperData()
            {
                Exercises = exercises
            };
        }
    }
}