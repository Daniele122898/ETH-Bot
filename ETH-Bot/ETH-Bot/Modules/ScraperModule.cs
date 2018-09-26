using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ETH_Bot.Data.Entities.SubEntities;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class ScraperModule : ModuleBase<SocketCommandContext>
    {
        private DownloadService _downloadService;
        private SemesterService _semesterService;

        public ScraperModule(DownloadService service, SemesterService semesterService)
        {
            _downloadService = service;
            _semesterService = semesterService;
        }

        [Command("available"), Alias("av")]
        public async Task AvailableScrapes()
        {
            var av = _semesterService.SemesterData;
            var eb = new EmbedBuilder()
            {
                Color = Utility.BlueInfoEmbed,
                Title = "All Available Semesters and Classes",
                Footer = Utility.RequestedBy(Context.User),
                ThumbnailUrl = Utility.EthLogo,
            };
            foreach (var semester in av)
            {
                string classes = "";
                semester.Lectures.ForEach(x=> classes += x.Name+"\n");
                if (string.IsNullOrWhiteSpace(classes)) continue;
                eb.AddField(x =>
                {
                    x.IsInline = false;
                    x.Name = semester.Name;
                    x.Value = classes;
                });
            }

            await ReplyAsync("", embed: eb.Build());
        }

        [Command("scrape"), Alias("s")]
        public async Task Scrape(string semester, [Remainder] string course)
        {
            course = course.Trim();
            // find shit in saved jsons
            var av = _semesterService.SemesterData;
            bool foundSem = false;
            bool foundClass = false;
            foreach (var semester1 in av)
            {
                if (!semester1.Name.Equals(semester, StringComparison.OrdinalIgnoreCase)) continue;
                foundSem = true;
                foreach (var lecture in semester1.Lectures)
                {
                    if (!lecture.Name.Equals(course, StringComparison.OrdinalIgnoreCase) && lecture.Alias.All(x=> !x.Equals(course, StringComparison.OrdinalIgnoreCase) )) 
                        continue;

                    foundClass = true;
                    // scrape class
                    ScraperData data = new ScraperData();
                    try
                    {
                        data = ScraperService.Scrape(lecture.Url, lecture.Xpath, lecture.Exercise, lecture.Solution,
                            lecture.HasExercise, lecture.HasSolution);
                    }
                    catch (Exception)
                    {
                        await ReplyAsync("", embed: Utility.ResultFeedback(
                                Utility.RedFailiureEmbed,
                                Utility.SuccessLevelEmoji[2], 
                                "The xPath or URL is wrong in the JSON file. Please report!")
                            .Build());
                        return;
                    }
                    
                    var eb = new EmbedBuilder()
                    {
                        Color = Utility.ETHBlue,
                        Title = lecture.Name,
                        Footer = Utility.RequestedBy(Context.User),
                        Description = "These are all the exercises and solutions.",
                        ThumbnailUrl = Utility.EthLogo,
                        Url = lecture.Url
                    };
                    
                    for (int i = 0; i < data.Exercises.Count; i++)
                    {
                        // since its possible to have a link without href if ur a fucking retarded french baguette eating idiot.
                        var href = data.Exercises[i].Attributes["href"]?.Value;
                        if (href == null) continue;
                        var exLink = (lecture.Relative ? 
                                         (string.IsNullOrWhiteSpace(lecture.Addition) ? 
                                             lecture.Url : 
                                             lecture.Url.Replace(lecture.Addition, "")) : "") 
                                     + href;
                        string solLink = null;
                        if (i < data.Solutions.Count)
                        {
                            href = data.Solutions[i].Attributes["href"]?.Value;
                            if (href != null)
                            {
                                solLink = (lecture.Relative
                                              ? (string.IsNullOrWhiteSpace(lecture.Addition)
                                                  ? lecture.Url
                                                  : lecture.Url.Replace(lecture.Addition, ""))
                                              : "")
                                          + href;
                            }
                        }
                        eb.AddField(x =>
                        {
                            x.Name = $"Exercise {i}";
                            x.IsInline = true;
                            x.Value = $"[View Exercise]({exLink}){(string.IsNullOrWhiteSpace(solLink) ? "" :$"\n[View Solution]({solLink})")}";
                        });
                    }

                    await ReplyAsync("", embed: eb.Build());
                }
            }

            if (!foundSem || !foundClass)
            {
                await ReplyAsync("", embed: Utility.ResultFeedback(
                    Utility.RedFailiureEmbed,
                    Utility.SuccessLevelEmoji[2], 
                    "Couldn't find Semester or Lecture!")
                .Build());
            }
        }
    }
}
