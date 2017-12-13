using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ETH_Bot.Data;
using ETH_Bot.Data.Entities;
using ETH_Bot.Data.Entities.SubEntities;
using HtmlAgilityPack;
using Microsoft.Extensions.DependencyInjection;

namespace ETH_Bot.Services
{
    public class SubscribeService
    {
        private Timer _timer;
        private DiscordSocketClient _client;
        
        private const int INITIAL_DELAY = 10;
        private const int MINUTE_INTERVAL = 10;


        public SubscribeService(DiscordSocketClient client)
        {
            _client = client;
        }

        public void Initialize()
        {
            SetTimer();
        }
        
        private void SetTimer()
        {
            _timer = new Timer(CheckUpdate, null, TimeSpan.FromSeconds(INITIAL_DELAY),
                TimeSpan.FromMinutes(MINUTE_INTERVAL));
        }

        public async Task Subscribe(SocketCommandContext context)
        {
            User user;
            using (var ethContext = new EthContext())
            {
                user = Utility.GetOrCreateUser(context.User.Id, ethContext);
                user.Subscribed = !user.Subscribed;
                await ethContext.SaveChangesAsync();
            }
            await context.Channel.SendMessageAsync("", embed: Utility.ResultFeedback(Utility.GreenSuccessEmbed, Utility.SuccessLevelEmoji[0],
                $"Successfully set Subscriber status to {(user.Subscribed ? "TRUE": "FALSE")}"));

        }

        private async void CheckUpdate(Object stateInfo)
        {
            try
            {
                using (var ethContext = new EthContext())
                {
                    var subbedUsers = ethContext.Users.Where(x => x.Subscribed).ToList();
                    if(subbedUsers.Count < 1)
                        return;
                    
                    //data
                    ScraperData linAlgFound = new ScraperData()
                    {
                        Exercises = new List<HtmlNode>(),
                        Solutions = new List<HtmlNode>()
                    };
                    ScraperData discMathFound = new ScraperData(){
                        Exercises = new List<HtmlNode>(),
                        Solutions = new List<HtmlNode>()
                    };
                    ScraperData eprogFound = new ScraperData(){
                        Exercises = new List<HtmlNode>()
                    };
                    ScraperData algDatFound = new ScraperData(){
                        Exercises = new List<HtmlNode>(),
                        Solutions = new List<HtmlNode>()
                    };
                    
                    //linalg
                    string linAlgUrl = "http://igl.ethz.ch/teaching/linear-algebra/la2017/";
                    var linAlg = ScraperService.ScrapeLinAlg(linAlgUrl);
                    foreach (var algExercise in linAlg.Exercises)
                    {
                        string href = linAlgUrl + algExercise.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.LinAlg.Any(x => x.Href == href))
                        {
                            //add to linalg list
                            linAlgFound.Exercises.Add(algExercise);
                            //add to context
                            ethContext.LinAlg.Add(new LinAlg()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }   
                    }
    
                    foreach (var algSolution in linAlg.Solutions)
                    {
                        string href = linAlgUrl + algSolution.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.LinAlg.Any(x => x.Href == href))
                        {
                            //add to linalg list
                            linAlgFound.Solutions.Add(algSolution);
                            //add to context
                            ethContext.LinAlg.Add(new LinAlg()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }   
                    }
                    
                    //discMath
                    string discMathUrl = "http://www.crypto.ethz.ch/teaching/lectures/DM17/";
                    var discMath = ScraperService.ScrapeDiscMath(discMathUrl);
                    foreach (var discMathExercise in discMath.Exercises)
                    {
                        string href = discMathUrl+discMathExercise.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.DiscMath.Any(x => x.Href == href))
                        {
                            //add to discmath list
                            discMathFound.Exercises.Add(discMathExercise);
                            //add to context
                            ethContext.DiscMath.Add(new DiscMath()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }
                    }
                    
                    foreach (var discMathSolution in discMath.Solutions)
                    {
                        string href = discMathUrl+discMathSolution.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.DiscMath.Any(x => x.Href == href))
                        {
                            //add to discmath list
                            discMathFound.Solutions.Add(discMathSolution);
                            //add to context
                            ethContext.DiscMath.Add(new DiscMath()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }
                    }
                    
                    //Eprog
                    var eprog = ScraperService.ScrapeEprog();
                    foreach (var eprogExercise in eprog.Exercises)
                    {
                        string href = eprogExercise.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.Eprog.Any(x => x.Href == href))
                        {
                            //add to discmath list
                            eprogFound.Exercises.Add(eprogExercise);
                            //add to context
                            ethContext.Eprog.Add(new Eprog()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }
                    }
                    
                    //algdat
                    string algDatUrl = "https://www.cadmo.ethz.ch/education/lectures/HS17/DA/";
                    var algDat = ScraperService.ScrapeAlgDat(algDatUrl);
                    
                    foreach (var algDatExercise in algDat.Exercises)
                    {
                        string href = algDatUrl+algDatExercise.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.AlgDat.Any(x => x.Href == href))
                        {
                            //add to discmath list
                            algDatFound.Exercises.Add(algDatExercise);
                            //add to context
                            ethContext.AlgDat.Add(new AlgDat()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }
                    }
                    
                    foreach (var algDatSolution in algDat.Solutions)
                    {
                        string href = algDatUrl+algDatSolution.Attributes["href"].Value;
                        //Doesnt exist
                        if (!ethContext.AlgDat.Any(x => x.Href == href))
                        {
                            //add to discmath list
                            algDatFound.Solutions.Add(algDatSolution);
                            //add to context
                            ethContext.AlgDat.Add(new AlgDat()
                            {
                                Href = href
                            });
                            await ethContext.SaveChangesAsync();
                        }
                    }
                    
                    //end of scrape
                    //prepare message
                    if (linAlgFound.Exercises.Count == 0 && linAlgFound.Solutions.Count == 0 &&
                        discMathFound.Exercises.Count == 0 && discMathFound.Solutions.Count == 0 &&
                        eprogFound.Exercises.Count == 0 &&
                        algDatFound.Exercises.Count == 0 && algDatFound.Solutions.Count == 0)
                    {
                        return;
                    }
                    var eb = new EmbedBuilder()
                    {
                        Color = Utility.ETHBlue,
                        ThumbnailUrl = Utility.EthLogo
                    };
    
                    int foundTypes = 0;
    
                    //found linalg update
                    if (linAlgFound.Exercises.Count > 0 || linAlgFound.Solutions.Count > 0)
                    {
                        foundTypes++;
                        eb.Title = "⏰ Lin Alg Alert";
                        string value = "";
                        if (linAlgFound.Exercises.Count > 0)
                        {
                            value += "**New Exercise**\n";
                            foreach (var exercise in linAlgFound.Exercises)
                            {
                                int exNumer = linAlg.Exercises.IndexOf(exercise)+1;
                                value += $"[View Exercise {exNumer}]({linAlgUrl}{exercise.Attributes["href"].Value})\n";
                            }
                        }
                        if (linAlgFound.Solutions.Count > 0)
                        {
                            value += "**New Solution**\n";
                            foreach (var solution in linAlgFound.Solutions)
                            {
                                int solNumer = linAlg.Solutions.IndexOf(solution)+1;
                                value += $"[View Solution {solNumer}]({linAlgUrl}{solution.Attributes["href"].Value})\n";
                            }
                        }
                        eb.AddField(x =>
                        {
                            x.IsInline = true;
                            x.Name = "Lineal Algebra";
                            x.Value = value;
                        });
                    }
                    
                    //found discmath update
                    if (discMathFound.Exercises.Count > 0 || discMathFound.Solutions.Count > 0)
                    {
                        foundTypes++;
                        eb.Title = "⏰ Disc Math Alert";
                        string value = "";
                        if (discMathFound.Exercises.Count > 0)
                        {
                            value += "**New Exercise**\n";
                            foreach (var exercise in discMathFound.Exercises)
                            {
                                int exNumer = discMath.Exercises.IndexOf(exercise)+1;
                                value += $"[View Exercise {exNumer}]({discMathUrl}{exercise.Attributes["href"].Value})\n";
                            }
                        }
                        if (discMathFound.Solutions.Count > 0)
                        {
                            value += "**New Solution**\n";
                            foreach (var solution in discMathFound.Solutions)
                            {
                                int solNumer = discMath.Solutions.IndexOf(solution)+1;
                                value += $"[View Solution {solNumer}]({discMathUrl}{solution.Attributes["href"].Value})\n";
                            }
                        }
                        eb.AddField(x =>
                        {
                            x.IsInline = true;
                            x.Name = "Discrete Math";
                            x.Value = value;
                        });
                    }
                    
                    //found algdat update
                    if (algDatFound.Exercises.Count > 0 || algDatFound.Solutions.Count > 0)
                    {
                        foundTypes++;
                        eb.Title = "⏰ A & D Alert";
                        string value = "";
                        if (algDatFound.Exercises.Count > 0)
                        {
                            value += "**New Exercise**\n";
                            foreach (var exercise in algDatFound.Exercises)
                            {
                                int exNumer = algDat.Exercises.IndexOf(exercise);
                                value += $"[View Exercise {exNumer}]({algDatUrl}{exercise.Attributes["href"].Value})\n";
                            }
                        }
                        if (algDatFound.Solutions.Count > 0)
                        {
                            value += "**New Solution**\n";
                            foreach (var solution in algDatFound.Solutions)
                            {
                                int solNumer = algDat.Solutions.IndexOf(solution);
                                value += $"[View Solution {solNumer}]({algDatUrl}{solution.Attributes["href"].Value})\n";
                            }
                        }
                        eb.AddField(x =>
                        {
                            x.IsInline = true;
                            x.Name = "A & D";
                            x.Value = value;
                        });
                    }
                    
                    //found eprog update
                    if (eprogFound.Exercises.Count > 0)
                    {
                        foundTypes++;
                        eb.Title = "⏰ Eprog Alert";
                        string value = "";
                        if (eprogFound.Exercises.Count > 0)
                        {
                            value += "**New Exercise**\n";
                            foreach (var exercise in eprogFound.Exercises)
                            {
                                int exNumer = eprog.Exercises.IndexOf(exercise);
                                value += $"[View Exercise {exNumer}]({exercise.Attributes["href"].Value})\n";
                            }
                        }
                        eb.AddField(x =>
                        {
                            x.IsInline = true;
                            x.Name = "Eprog";
                            x.Value = value;
                        });
                    }
                    
                    //If more then 1 type of lecture was released then change the title 
                    if (foundTypes > 1)
                    {
                        eb.Title = "⏰ Subscriber Alert";
                    }
                    
                    //Send Message to all subscribed users
                    foreach (var user in subbedUsers)
                    {
                        var userToNotify = _client.GetUser(user.UserId);
                        if(userToNotify == null)
                            continue;
                        await (await userToNotify.GetOrCreateDMChannelAsync()).SendMessageAsync("", embed: eb);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}