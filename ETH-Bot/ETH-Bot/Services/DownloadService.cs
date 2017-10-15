using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using HtmlAgilityPack;

namespace ETH_Bot.Services
{
    public class DownloadService
    {

        public async Task DownloadLinAlg(SocketCommandContext context)
        {
            string url = "http://igl.ethz.ch/teaching/linear-algebra/la2017/";
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

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Linear Algebra",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions",
                ThumbnailUrl = Utility.EthLogo
            };

            for (int i = 0; i < exercises.Count; i++)
            {
                var exLink = url+exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < solutions.Count)
                {
                    solLink = url+solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb);
        }

        public async Task DownloadEprog(SocketCommandContext context)
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
            
            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Introduction to Programming",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions. Since this scraper does not include the extra materials [click here to view them](http://www.lst.inf.ethz.ch/education/einfuehrung-in-die-programmierung-i--252-0027-.html)",
                ThumbnailUrl = Utility.EthLogo
            };

            for (int i = 0; i < exercises.Count; i++)
            {
                var exLink = exercises[i].Attributes["href"].Value;
                
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink})";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb);
        }

        public async Task DownloadAlgDat(SocketCommandContext context)
        {
            string url = "https://www.cadmo.ethz.ch/education/lectures/HS17/DA/";
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

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Algorithms and Datastructures",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions",
                ThumbnailUrl = Utility.EthLogo
            };

            for (int i = 0; i < exercises.Count; i++)
            {
                var exLink = url+exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < solutions.Count)
                {
                    solLink = url+solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb);
        }
        public async Task DownloadDiscMath(SocketCommandContext context)
        {
            string url = "http://www.crypto.ethz.ch/teaching/lectures/DM17/";
            var web = new HtmlWeb();
            var doc = web.Load(url);
            //var node = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/div[2]/table[1]/tbody/tr[5]/td[4]/a"); tbody might break shit
            var nodes = doc.DocumentNode.SelectNodes("/html/body/div/div[2]/div[2]/table[1]//tr//td//a"); // double slash what you want to completely search
            /*
            string innerText = "";
            string href = "";
            
            foreach (var node in nodes)
            {
                innerText += $"{node.InnerText} \n";
                href += $"{url+node.Attributes["href"].Value} \n";
            }
            await context.Channel.SendMessageAsync($"**Inner Text:**\n {innerText}\n" +
                                                   $"**HREF:** {href}");*/
            
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

            var eb = new EmbedBuilder()
            {
                Color = Utility.ETHBlue,
                Title = "Discrete Math",
                Footer = Utility.RequestedBy(context.User),
                Description = "These are all the exercises and solutions",
                ThumbnailUrl = Utility.EthLogo
            };

            for (int i = 0; i < exercises.Count; i++)
            {
                //href += $"{url+node.Attributes["href"].Value} \n";
                var exLink = url+exercises[i].Attributes["href"].Value;
                string solLink = null;
                if (i < solutions.Count)
                {
                    solLink = url+solutions[i].Attributes["href"].Value;
                }
                eb.AddField(x =>
                {
                    x.Name = $"Exercise {i+1}";
                    x.IsInline = true;
                    x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                });
            }

            await context.Channel.SendMessageAsync("", embed: eb);
        }
    }
}