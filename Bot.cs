using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

using Newtonsoft.Json;
using BotTemplate.Objects.Sql.Profile;
using BotTemplate.Managers;
using BotTemplate.Objects.Json;

namespace BotTemplate
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public static DiscordClient client2 { get; set; }
        public BotConfig Config { get; private set; }
        public Database Db { get; private set; }
        public CancellationTokenSource CancellationTokenSource { get; private set; }

        private void Initialize()
        {
            /* Your config file */
            this.Config = JsonConvert.DeserializeObject<BotConfig>(File.ReadAllText("./Resources/config.json"));

            this.Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = this.Config.Token,
                TokenType = TokenType.Bot,

                UseInternalLogHandler = true
            });
            Bot.client2 = this.Client;
        }

        private void PostInitialize()
        {
            this.Client.Ready += (e) =>
            {
                this.Client.DebugLogger.LogMessage(LogLevel.Info, nameof(this.Client), "Ready", DateTime.Now);
                return Task.CompletedTask;
            };

            this.CancellationTokenSource = new CancellationTokenSource();

            AppDomain.CurrentDomain.ProcessExit += (s, e)
                => this.CancellationTokenSource.Cancel();

            Console.CancelKeyPress += (s, e)
                => this.CancellationTokenSource.Cancel();

            /* This way you can access the Bot object without globals */
            var deps = new ServiceCollection()
                .AddSingleton<Bot>(f => this)
                .BuildServiceProvider();
            var cnext = this.Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = this.Config.Prefixes,
                CaseSensitive = false,
                IgnoreExtraArguments = true,
                Services = deps
            });

            /* Basic command handling */
            cnext.CommandErrored += async (e) =>
            {
                var et = e.Exception.GetType();
                if (et == typeof(CommandNotFoundException))
                    return;
                if (et == typeof(UserException))
                {
                    await e.Context.RespondAsync(e.Exception.Message);
                }
                else
                {
                    if (et == typeof(ArgumentException)
                        && e.Exception.Message == "Could not find a suitable overload for the command.")
                    {
                        await e.Context.RespondAsync($"Invalid syntax for `{e.Command.QualifiedName}`. See `{e.Context.Prefix}help {e.Command.QualifiedName}` for info on that.");
                        return;
                    }
                    if (e.Exception.Message == "No matching subcommands were found, and this group is not executable.")
                    {
                        await e.Context.RespondAsync($"You either spelled the command wrong or it doesn't exist in specified group.");
                        return;
                    }
                    if (e.Exception.GetType() == typeof(ChecksFailedException))
                    {
                        var ce = (ChecksFailedException)e.Exception;
                        foreach (var failed in ce.FailedChecks)
                        {
                            if (failed.GetType() == typeof(RequirePermissionsAttribute))
                            {
                                // permission failed
                                await e.Context.RespondAsync("You do not have permission to use this command.");
                                return;
                            }
                            else if (failed.GetType() == typeof(CooldownAttribute))
                            {
                                if (e.Exception.Message == "One or more pre-execution checks failed.")
                                {
                                    var time = e.Command.GetType().GetCustomAttributes(typeof(CooldownAttribute), true);
                                    if (time != null)
                                    {
                                        await e.Context.RespondAsync($"The command is on cooldown. {((e.Command.ExecutionChecks[0] as CooldownAttribute).GetBucket(e.Context).ResetsAt - DateTimeOffset.UtcNow).TotalSeconds.ToString("N0")} seconds left");
                                    }
                                    return;
                                }
                            }
                        }
                    }
                    await e.Context.RespondAsync($"error: ```{e.Exception}```");
                }
            };
            cnext.RegisterCommands(System.Reflection.Assembly.GetExecutingAssembly());

            this.Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromHours(2)
            });

            /* Put Db initialization code here */
            this.Db = new Database("./Resources/database.db");
            ManageCharacter.Initialize(this.Db);
            Helpers.Owners.InitCache();
            Helpers.TimerChr.Init(this.Db);


            /*XP System*/
            this.Client.MessageCreated += async (e) =>
            {
                if (e.Guild == null)
                    return;
                if (e.Author.IsBot)
                    return;

                Console.WriteLine($"${e.Guild.Name} | #{e.Channel.Name} | @{e.Author.Username + "#" + e.Author.Discriminator} → {e.Message.Content}");
                if (e.Channel.Name.StartsWith("darii"))
                {
                    if (Helpers.Tools.RNG.Next(5) != 0)
                        return;
                    await e.Message.CreateReactionAsync(DiscordEmoji.FromUnicode(client2, "😢"));
                    return;
                }
                var CH = await ManageCharacter.GetChannel(e.Channel.Id);
                if (CH == null)
                    return;
                if (CH.Category != ChannelCategory.Rp)
                    return;
                int XP = e.Message.Content.Count() / 100 + CH.XP;
                if (XP > 5)
                {
                    var Accg = new AccountGet()
                    {
                        UID = e.Author.Id,
                        GID = e.Guild.Id
                    };
                    var Check = await ManageCharacter.GetAll(Acc: Accg);
                    if (Check == null)
                        return;
                    var OwnGet = new OwnerGet()
                    {
                        UID = e.Author.Id,
                        GID = e.Guild.Id,
                        Slot = Check.Slot
                    };
                    var Own = await ManageCharacter.GetAll(OwnGet);
                    var ChrGet = new ChrGet()
                    {
                        Entry = Own.CharEntry,
                    };
                    var Chr = await ManageCharacter.GetAll(ChrGet);
                    var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, "xp");
                    if (Cdr != null)
                    {
                        return;
                    }
                    XP += Helpers.Tools.RNG.Next(20) * Chr.Level;
                    XP = XP*2;
                    Chr.XP += XP;
                    DiscordMember mem = await e.Guild.GetMemberAsync(e.Author.Id);
                    if (Chr.XP >= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level))
                    {
                        var olvl = Chr.Level;
                    begin:
                        Chr.XP -= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level);
                        Chr.Level += 1;
                        if(Chr.Level > 10 && Chr.Level < 15)
                        Chr.SP += 4;
                        else if(Chr.Level > 15 && Chr.Level < 20)
                        Chr.SP += 3;
                        else if(Chr.Level > 20)
                        Chr.SP += 2;
                        else
                        Chr.SP += 5;
                        if(Chr.Level > 10)
                        Chr.UPoints += 1;
                        Chr.Balance += 60;
                        if (Chr.XP >= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level))
                            goto begin;
                        await ManageCharacter.UpdateAsync(Chr);
                        await mem.SendMessageAsync($"**{Chr.Name}** Leveled! `{olvl}` → `{Chr.Level}`");
                    }
                    else
                    await ManageCharacter.UpdateAsync(Chr);
                    await mem.SendMessageAsync($"**{Chr.Name}** earned `{XP}` **XP**");
                    await Helpers.TimerChr.AddCooldown(Chr.Entry, "xp", 120);
                }

            };
        }

        public async Task RunAsync()
        {
            this.Initialize();
            this.PostInitialize();

            try
            {
                await this.Client.ConnectAsync();
                await Helpers.TimerChr.LoadCooldown();
                await Task.Delay(-1, this.CancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
            }
            catch (Exception ex)
            {
                this.Client.DebugLogger.LogMessage(LogLevel.Critical, nameof(Bot), "Exception in main loop.", DateTime.Now,
                                                   ex);
            }

            this.Client.DebugLogger.LogMessage(LogLevel.Info, nameof(Bot), "Disconnecting...", DateTime.Now);
            await this.Client.DisconnectAsync();
        }
    }
}
