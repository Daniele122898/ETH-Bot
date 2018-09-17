using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using ETH_Bot.Services;

namespace ETH_Bot.Modules
{
    public class MiscModule : ModuleBase<SocketCommandContext>
    {
        [Command("ping")]
        public async Task Ping()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], $"Pong! {Context.Client.Latency} ms :ping_pong:").Build());
        }

        [Command("github"), Alias("git")]
        public async Task GitHub()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], $"Find me here").WithUrl("https://github.com/Daniele122898/ETH-Bot").Build());
        }
        
        [Command("sys"), Alias("info"), Summary("Gives stats about ETH-Bot")]
        public async Task GetSysInfo()
        {
            var proc = Process.GetCurrentProcess();

            long FormatRamValue(long d)
            {
                while (d > 1024)
                {
                    d /= 1024;
                }
                return d;
            }

            string FormatRamUnit(long d)
            {
                var units = new string[] {"B", "KB", "MB", "GB", "TB", "PB"};
                var unitCount = 0;
                while (d > 1024)
                {
                    d /= 1024;
                    unitCount++;
                }
                return units[unitCount];
            }

            var eb = new EmbedBuilder()
            {
                Color = Utility.BlueInfoEmbed,
                ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                Footer = Utility.RequestedBy(Context.User),
                Title = $"{Utility.SuccessLevelEmoji[3]} **ETH-Bot Sys Info**",
                Url = "https://github.com/Daniele122898/ETH-Bot"
            };
            eb.AddField((x) =>
            {
                x.Name = "Uptime";
                x.IsInline = true;
                x.Value = (DateTime.Now - proc.StartTime).ToString(@"d'd 'hh\:mm\:ss");
            });
            eb.AddField((x) =>
            {
                x.Name = "Used RAM";
                x.IsInline = true;
                var mem = GC.GetTotalMemory(false);
                x.Value = $"{FormatRamValue(mem):f2} {FormatRamUnit(mem)} / {FormatRamValue(proc.WorkingSet64):f2} {FormatRamUnit(proc.WorkingSet64)}";
            });
            eb.AddField((x) =>
            {
                x.Name = "Ping";
                x.IsInline = true;
                x.Value = $"{Context.Client.Latency} ms";
            });
            eb.AddField((x) =>
            {
                x.Name = "Version";
                x.IsInline = true;
                x.Value = $"{ConfigService.GetConfigData("version")}";
            });
            await ReplyAsync("", embed: eb.Build());
        }


        [Command("help")]
        public async Task Help()
        {
            await ReplyAsync("", embed: Utility.ResultFeedback(Utility.ETHBlue, Utility.SuccessLevelEmoji[4], "Simple Help").AddField(
                x =>
                {
                    x.IsInline = false;
                    x.Name = "Scraper";
                    x.Value = "`>linalg`\n" +
                              "`>discmath`\n" +
                              "`>algdat`\n" +
                              "`>eprog`\n" +
                              "`>sub`";
                }).AddField(x =>
            {
                x.IsInline = false;
                x.Name = "Other";
                x.Value = "`>ping`\n" +
                          "`>github`";
            }).AddField(x =>
            {
                x.IsInline = false;
                x.Name = "Reminder";
                x.Value = "[Read this but use > as prefix](http://git.argus.moe/serenity/SoraBot-v2/wikis/Commands/reminders)";
            }).WithThumbnailUrl(Utility.EthLogo).Build());
        }
    }
}