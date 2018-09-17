﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;
using Discord.WebSocket;
using ETH_Bot.Data;
using ETH_Bot.Data.Entities.SubEntities;
using Microsoft.Extensions.DependencyInjection;

namespace ETH_Bot.Services
{
    public class ReminderService
    {
        private Timer _timer;
        private DiscordSocketClient _client;
        private InteractiveService _interactive;

        public ReminderService(DiscordSocketClient client, InteractiveService interactiveService)
        {
            _client = client;
            _interactive = interactiveService;
        }
        
        public void Initialize()
        {
            SetTimer();
        }
        
        private const int INITIAL_DELAY = 40;
        
        private void SetTimer()
        {
            _timer = new Timer(CheckReminders, null, TimeSpan.FromSeconds(INITIAL_DELAY),
                TimeSpan.FromSeconds(INITIAL_DELAY));
            //ChangeToClosestInterval();
        }

        public async Task ShowReminders(SocketCommandContext context)
        {
            using (var ethContext = new EthContext())
            {
                var userDb = Utility.OnlyGetUser(context.User.Id, ethContext);
                if (userDb == null || userDb.Reminders.Count == 0)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                            "You have no reminders!").Build());
                    return;
                }

                var eb = new EmbedBuilder()
                {
                    Color = Utility.PurpleEmbed,
                    Title = "Reminders ⏰",
                    ThumbnailUrl = context.User.GetAvatarUrl() ?? Utility.StandardDiscordAvatar
                };

                var sortedReminders = userDb.Reminders.OrderBy(x => x.Time).ToList();

                for (int i = 0; i < (sortedReminders.Count > 10 ? 10 : sortedReminders.Count); i++)
                {
                    eb.AddField(x =>
                    {
                        x.Name =
                            $"Reminder #{i + 1} in {ConvertTime(sortedReminders[i].Time.Subtract(DateTime.UtcNow).TotalSeconds)}";
                        x.IsInline = false;
                        x.Value =
                            $"{(sortedReminders[i].Message.Length > 80 ? sortedReminders[i].Message.Remove(80) + "..." : sortedReminders[i].Message)}";
                    });
                }

                await context.Channel.SendMessageAsync("", embed: eb.Build());
            }
        }

        public async Task RemoveReminder(SocketCommandContext context)
        {
            using (var ethContext = new EthContext())
            {
                var userDb = Utility.OnlyGetUser(context.User.Id, ethContext);
                if (userDb == null || userDb.Reminders.Count == 0)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                            "You have no reminders!").Build());
                    return;
                }

                var eb = new EmbedBuilder()
                {
                    Color = Utility.PurpleEmbed,
                    Title = "Enter index of reminder to remove",
                    ThumbnailUrl = context.User.GetAvatarUrl() ?? Utility.StandardDiscordAvatar
                };
                var orderedReminders = ethContext.Reminders.Where(x => x.UserForeignId == context.User.Id).ToList();
                for (int i = 0; i < (orderedReminders.Count > 24 ? 24 : orderedReminders.Count); i++)
                {
                    eb.AddField(x =>
                    {
                        x.Name =
                            $"Reminder #{i + 1} in {ConvertTime(orderedReminders[i].Time.Subtract(DateTime.UtcNow).TotalSeconds)}";
                        x.IsInline = false;
                        x.Value =
                            $"{(orderedReminders[i].Message.Length > 80 ? orderedReminders[i].Message.Remove(80) + "..." : orderedReminders[i].Message)}";
                    });
                }
                var msg = await context.Channel.SendMessageAsync("", embed: eb.Build());
                //var response = await _interactive.WaitForMessage(context.User, context.Channel, TimeSpan.FromSeconds(45));
                
                var response = await _interactive.NextMessageAsync(context, true, true, TimeSpan.FromSeconds(45));//TODO test if this listens only to source user and source channel
                await msg.DeleteAsync();
                if (response == null)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                            $"{Utility.GiveUsernameDiscrimComb(context.User)} didn't reply in time :<").Build());
                    return;
                }
                int index = 0;
                if (!Int32.TryParse(response.Content, out index))
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                            "Only add the Index!").Build());
                    return;
                }
                if (index > (orderedReminders.Count + 1) || index < 1)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(Utility.RedFailiureEmbed, Utility.SuccessLevelEmoji[2],
                            "Invalid Number").Build());
                    return;
                }
                index -= 1;
                ethContext.Reminders.Remove(orderedReminders[index]);
                await ethContext.SaveChangesAsync();
                ChangeToClosestInterval();
                await context.Channel.SendMessageAsync("",
                    embed: Utility.ResultFeedback(Utility.GreenSuccessEmbed, Utility.SuccessLevelEmoji[0],
                        "Successfully removed reminder").Build());
            }
        }
        
        private string ConvertTime(double value)
        {
            TimeSpan ts = TimeSpan.FromSeconds(value);
            if (value > 86400)
            {
                return $"{ts.Days}d {ts.Hours}h {ts.Minutes}m {ts.Seconds:D2}s";
            }
            else if (value > 3600)
            {
                return string.Format("{0}h {1}m {2:D2}s", ts.Hours, ts.Minutes, ts.Seconds);
            }
            else if (value > 60)
            {
                return string.Format("{0}m {1:D2}s", ts.Minutes, ts.Seconds);
            }
            return string.Format("{0:D2}s", ts.Seconds);
        }
        
        private async void CheckReminders(Object stateInfo)
        {
            using (var ethContext = new EthContext())
            {
                List<Reminder> rems = new List<Reminder>();
                rems = ethContext.Reminders.ToList();
                bool triggered = false;
                foreach (var reminder in rems)
                {
                    if (reminder.Time.CompareTo(DateTime.UtcNow) <= 0)
                    {
                        triggered = true;
                        var userToRemind = _client.GetUser(reminder.UserForeignId);
                        if (userToRemind == null)
                        {
                            ethContext.Reminders.Remove(reminder);
                            continue;
                        }
                        await (await userToRemind.GetOrCreateDMChannelAsync()).SendMessageAsync("",
                            embed: Utility
                                .ResultFeedback(Utility.PurpleEmbed, Utility.SuccessLevelEmoji[4], $"**Reminder** ⏰")
                                .WithDescription($"{reminder.Message}").Build());
                        ethContext.Reminders.Remove(reminder);
                    }
                }
                await ethContext.SaveChangesAsync();
                if (triggered)
                    ChangeToClosestInterval();
            }
        }

        public async Task SetReminder(SocketCommandContext context, string message)
        {
            var time = GetTime(message);
            using (var ethContext = new EthContext())
            {
                if (time == 0)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(
                            Utility.RedFailiureEmbed, 
                            Utility.SuccessLevelEmoji[2], 
                            "Either the message or the time format was incorrect!")
                            .WithDescription($"Example: >remind do this in 3 h 10mins")
                            .Build());
                    return;
                }
                if (time == -1)
                {
                    await context.Channel.SendMessageAsync("",
                        embed: Utility.ResultFeedback(
                            Utility.RedFailiureEmbed, 
                            Utility.SuccessLevelEmoji[2], 
                            "You need to add a message to your Reminder!")
                            .WithDescription($"Example: >remind do this in 3 h 10mins")
                            .Build());
                    return;
                }
            
                var userDb = Utility.GetOrCreateUser(context.User.Id, ethContext);

                var msg = message.Substring(0, message.LastIndexOf(" in ", StringComparison.OrdinalIgnoreCase));
                userDb.Reminders.Add(new Reminder()
                {
                    Message = msg,
                    Time = DateTime.UtcNow.AddSeconds(time),
                    //Id = _soraContext.Reminders.Count()+1
                });

                await ethContext.SaveChangesAsync();

                await context.Channel.SendMessageAsync("",
                    embed: Utility.ResultFeedback(Utility.GreenSuccessEmbed, Utility.SuccessLevelEmoji[0],
                            "Successfully set Reminder!")
                        .WithDescription(
                            $"I will remind you to `{msg}` in `{message.Substring(message.LastIndexOf(" in ", StringComparison.OrdinalIgnoreCase) + " in ".Length)}`!").Build());
                ChangeToClosestInterval();
            }
        }

        private double GetTime(string message)
        {
            if (!message.Contains(" in "))
            {
                return 0;
            }
            if(string.IsNullOrWhiteSpace(message.Substring(message.LastIndexOf(" in ", StringComparison.OrdinalIgnoreCase) + " in ".Length)))
            {
                return -1;
            }
            var msg = message.Substring(message.LastIndexOf(" in ", StringComparison.OrdinalIgnoreCase));//TODO + " in " lenght
            var regex = Regex.Matches(msg, @"(\d+)\s{0,1}([a-zA-Z]*)");
            double timeToAdd = 0;
            for (int i = 0; i < regex.Count; i++)
                {
                    var captures = regex[i].Groups;
                    if (captures.Count < 3)
                    {
                        Console.WriteLine("CAPTURES COUNT LESS THEN 3");
                        return 0;
                    }

                    double amount = 0;

                    if (!Double.TryParse(captures[1].ToString(), out amount))
                    {
                        Console.WriteLine($"COULDNT PARSE DOUBLE : {captures[1].ToString()}");
                        return 0;
                    }

                    switch (captures[2].ToString())
                    {
                        case ("weeks"):
                        case ("week"):
                        case ("w"):
                            timeToAdd += amount * 604800;
                            break;
                        case ("day"):
                        case ("days"):
                        case ("d"):
                            timeToAdd += amount * 86400;
                            break;
                        case ("hours"):
                        case ("hour"):
                        case ("h"):
                            timeToAdd += amount * 3600;
                            break;
                        case ("minutes"):
                        case ("minute"):
                        case ("m"):
                        case ("min"):
                        case ("mins"):
                            timeToAdd += amount * 60;
                            break;
                        case ("seconds"):
                        case ("second"):
                        case ("s"):
                            timeToAdd += amount;
                            break;
                        default:
                            Console.WriteLine("SWITCH FAILED");
                            return 0;
                    }
                }
                return timeToAdd;
        }
        

        private void ChangeToClosestInterval()
        {
            using (var ethContext = new EthContext())
            {
                if (ethContext.Reminders.ToList().Count == 0)
                {
                    _timer.Change(Timeout.Infinite, Timeout.Infinite);
                    Console.WriteLine("TIMER HAS BEEN HALTED!");
                    return;
                }

                var sortedReminders = ethContext.Reminders.ToList().OrderBy(x => x.Time).ToList();
                var time = sortedReminders[0].Time.Subtract(DateTime.UtcNow).TotalSeconds;
                if (time < 0)
                {
                    time = 0;
                }
                if (time > 86400)
                {
                    //just set timer to 1 day
                    _timer.Change(TimeSpan.FromDays(1), TimeSpan.FromDays(1));
                    Console.WriteLine($"CHANGED TIMER INTERVAL TO: 1 day bcs the timer was too long");
                }
                else
                {
                    _timer.Change(TimeSpan.FromSeconds(time), TimeSpan.FromSeconds(time));  
                    Console.WriteLine($"CHANGED TIMER INTERVAL TO: {time}");
                }
            }
        }

    }
}