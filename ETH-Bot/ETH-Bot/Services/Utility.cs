using System;
using System.Collections.Generic;
using System.Linq;
using Discord;
using Discord.WebSocket;
using ETH_Bot.Data;
using ETH_Bot.Data.Entities;
using ETH_Bot.Data.Entities.SubEntities;

namespace ETH_Bot.Services
{
    public static class Utility
    {
        public const ulong OWNER_ID = 192750776005689344;
        
        public static Discord.Color ETHBlue = new Color(0, 61, 122);
        public static Discord.Color PurpleEmbed = new Discord.Color(109, 41, 103);
        public static Discord.Color YellowWarningEmbed = new Color(255,204,77);
        public static Discord.Color GreenSuccessEmbed = new Color(119,178,85);
        public static Discord.Color RedFailiureEmbed = new Color(221,46,68);
        public static Discord.Color BlueInfoEmbed = new Color(59,136,195);
        public static string StandardDiscordAvatar = "http://i.imgur.com/tcpgezi.jpg";
        public static string EthLogo = "https://i.imgur.com/0TgePEu.png";
        public static string[] SuccessLevelEmoji = new string[]
        {
            "✅","⚠","❌","ℹ",""
        };
        
        public static EmbedBuilder ResultFeedback(Discord.Color color, string symbol, string text)
        {
            var eb = new EmbedBuilder()
            {
                Color = color,
                Title = $"{symbol} {text}"
            };
            return eb;
        }

        public static User OnlyGetUser(ulong id, EthContext ethContext)
        {
            var result = ethContext.Users.FirstOrDefault(x => x.UserId == id);
            if (result != null)
            {
                var reminders = ethContext.Reminders.Where(x => x.UserForeignId == id).ToList();
                result.Reminders = reminders;
            }
            return result;
        }

        public static User GetOrCreateUser(ulong id, EthContext ethContext)
        {
            User result = null;
            try
            {
                result = ethContext.Users.FirstOrDefault(x => x.UserId == id);
                if (result == null)
                {
                    //User not found
                    var addedUser = ethContext.Users.Add(new User() { UserId = id, Reminders = new List<Reminder>()});
                }

                var reminders = ethContext.Reminders.Where(x => x.UserForeignId == id).ToList();
                result.Reminders = reminders;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ethContext.SaveChanges();
            return result;
        }

        public static EmbedFooterBuilder RequestedBy(SocketUser user)
        {
            return new EmbedFooterBuilder()
            {
                Text = $"Requested by {Utility.GiveUsernameDiscrimComb(user)}",
                IconUrl = user.GetAvatarUrl() ?? StandardDiscordAvatar
            };
        }

        public static string GiveUsernameDiscrimComb(SocketUser user)
        {
            return user == null ? "User Unknown" : $"{user.Username}#{user.Discriminator}";
        }
    }
}