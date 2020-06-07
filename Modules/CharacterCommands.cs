using System;
using System.Threading.Tasks;
using System.Linq;
using System.IO;
using System.Net;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules
{

    [Group("Character"), Aliases(new string[] { "ch", "char" })]
    [Description("The Character Group")]
    public class Character : BaseCommandModule
    {
        private Random _rng = new Random();
        [GroupCommand()]
        public async Task ViewAsync(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);

            var emmy = await ManageCharacter.EmbedProfileAsync(Chr, ctx: ctx);
            await ctx.RespondAsync(embed: emmy);
        }
        [Command("Reference"), Aliases("Ref")]
        public async Task RefAsyn(CommandContext ctx, DiscordUser User = null)
        {
            User = User ?? ctx.User;
            var Accg = new AccountGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync($"{User.Mention} has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var emmy = new DiscordEmbedBuilder();
            emmy = emmy.WithImageUrl(Chr.Image);
            await ctx.RespondAsync(embed: emmy);
        }
        [Command("Theme")]
        public async Task ThemeAsyn(CommandContext ctx, DiscordUser User = null)
        {
            User = User ?? ctx.User;
            var Accg = new AccountGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync($"{User.Mention} has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var emmy = new DiscordEmbedBuilder();
            emmy = emmy.WithAuthor(Chr.Theme, Chr.Theme, Chr.Image);
            await ctx.RespondAsync(embed: emmy);
        }
        [Command("Train"), Aliases("T")]
        public async Task TrainAsyn(CommandContext ctx)
        {
            var User = ctx.User;
            var Accg = new AccountGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync($"{User.Mention} has no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, "train");
            if (Cdr != null)
            {
                await ctx.RespondAsync($"You have already Trained, wait for {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                return;
            }
            var XpGained = _rng.Next(Chr.Level * 15) + 10;
            Chr.XP += XpGained;
            if (Chr.XP >= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level))
            {
                var olvl = Chr.Level;
            begin:
                Chr.XP -= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level);
                Chr.Level += 1;
                if (Chr.Level > 10 && Chr.Level < 15)
                    Chr.SP += 4;
                else if (Chr.Level > 15 && Chr.Level < 20)
                    Chr.SP += 3;
                else if (Chr.Level > 20)
                    Chr.SP += 2;
                else
                    Chr.SP += 5;
                if (Chr.Level > 10)
                    Chr.UPoints += 1;
                Chr.Balance += 60;
                if (Chr.XP >= Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level))
                    goto begin;
                await ManageCharacter.UpdateAsync(Chr);
                await ctx.RespondAsync($"**{Chr.Name}** Leveled! `{olvl}` → `{Chr.Level}`");
            }
            else
                await ManageCharacter.UpdateAsync(Chr);
            await Helpers.TimerChr.AddCooldown(Chr.Entry, "train", 2400);
            await ctx.RespondAsync("You trained a little for " + XpGained + " xp!");
        }
        [Command("search")]
        [Description("Find a character by its name.")]
        public async Task SearchAsync(CommandContext ctx, [RemainingText, Description("Character name")] string name)
        {
            var Chr = await ManageCharacter.GetChrByName(name);
            if (Chr == null)
            {
                await ctx.RespondAsync("No character by that name.");
                return;
            }
            string Owner = "";
            var Own = await ManageCharacter.GetOwnerByEntry(Chr.Entry);
            if (Own == null)
            {
                Owner = "**From: No one**";
            }
            else
            {
                Owner = "**From: <@" + Own.UID + ">**";
            }
            var emmy = await ManageCharacter.EmbedProfileAsync(Chr, ctx, Owner);
            await ctx.RespondAsync(embed: emmy);
        }
        [Command("To")]
        [Description("Switch to another character by slot or name")]
        public async Task SlotAsync(CommandContext ctx, [Description("Slot number")] int Slot)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            if (Own == null)
            {
                await ctx.RespondAsync("You don't have a character on that slot.");
                return;
            }
            Check.Slot = Slot;
            await ManageCharacter.UpdateAsync(Account: Check);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            await ctx.RespondAsync($"**Switched to slot** `{Slot}` **||** `{Chr.Name}`");
        }
        [Command("To")]
        public async Task NameAsync(CommandContext ctx, [RemainingText, Description("Character name")] string name)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };

            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }

            var Slot = await ManageCharacter.GetChrByName(name);
            if (Slot == null)
            {
                await ctx.RespondAsync("No character by that name.");
                return;
            }

            var Own = await ManageCharacter.GetOwnerByEntry(Slot.Entry);
            if (Own == null)
            {
                await ctx.RespondAsync("No one owns this character.");
                return;
            }

            if (Own.UID != ctx.User.Id)
            {
                await ctx.RespondAsync("You do not own this character.");
                return;
            }

            if (Own.GID != ctx.Guild.Id)
            {
                await ctx.RespondAsync("This character currently doesn't belong to this guild.");
                return;
            }

            Check.Slot = Own.Slot;
            await ManageCharacter.UpdateAsync(Account: Check);

            await ctx.RespondAsync($"**Switched to ** `{Check.Slot}` **||** `{Slot.Name}`");
        }
        [Command("List"), Aliases("ls"), Description("List all your characters.")]
        public async Task ListAsync(CommandContext ctx, DiscordUser Target)
        {
            var Accg = new AccountGet()
            {
                UID = Target.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync($"{Target.Mention} has no characters.");
                return;
            }
            int i = 0;
            var sb = new StringBuilder();
            sb.Append("`Characters:`\n");
            while (i < 50)
            {
                var OwnGet = new OwnerGet()
                {
                    UID = Target.Id,
                    GID = ctx.Guild.Id,
                    Slot = i
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                if (Own == null)
                {
                    i += 100;
                }
                else
                {
                    var ChrGet = new ChrGet()
                    {
                        Entry = Own.CharEntry,
                    };
                    var Chr = await ManageCharacter.GetAll(ChrGet);
                    sb.Append($"`[{i}]` **→** **[__{Chr.Class}__ - __{Chr.Name}__ - __{Chr.Race}__]**  [__Level:__ `{Chr.Level}`] **→** [__XP__ `{Chr.XP}|{Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level)}`]\n\n");
                    i++;
                }
            }
            var embed = new DiscordEmbedBuilder();
            var inter = ctx.Client.GetInteractivity();
            var pages = inter.GeneratePagesInEmbed(sb.ToString(), SplitType.Line, embed);
            await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
        [Group("Marriage"), Aliases("marry")]
        [Description("Propose to your love-")]
        public class Marriage : BaseCommandModule
        {
            [GroupCommand]
            public async Task MainAsync(CommandContext ctx, [Description("Victi- i mean loved one.")] DiscordUser User)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                var Accg2 = new AccountGet()
                {
                    UID = User.Id,
                    GID = ctx.Guild.Id
                };
                var Check2 = await ManageCharacter.GetAll(Acc: Accg2);
                if (Check2 == null)
                {
                    await ctx.RespondAsync("Victim has no character.");
                    return;
                }
                var OwnGet2 = new OwnerGet()
                {
                    UID = User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check2.Slot
                };
                var Own2 = await ManageCharacter.GetAll(OwnGet2);
                var ChrGet2 = new ChrGet()
                {
                    Entry = Own2.CharEntry,
                };
                var Chr2 = await ManageCharacter.GetAll(ChrGet2);
                DiscordUser Victim = User;
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"<@{Chr2.Name}>, Do you accept <@{Chr.Name}>'s request to marry?", 120000, ctx.Channel, Victim);
                if (r == null)
                {
                    await ctx.RespondAsync("Failed, no response found.");
                    return;
                }
                if (r.Content.ToLower().StartsWith("ye"))
                {
                    await ctx.RespondAsync($"{Chr.Name}, you may kiss your (Husband/Wife) {Chr2.Name}");
                    if (Chr.Marriage != 0)
                    {
                        var Chrg = new ChrGet()
                        {
                            Entry = Chr.Marriage,
                        };
                        var Chrs = await ManageCharacter.GetAll(Chrg);
                        Chrs.Marriage = 0;
                        await ManageCharacter.UpdateAsync(Chrs);
                        await ctx.RespondAsync($"{Chr.Name} divorced from {Chrs.Name}");
                    }
                    if (Chr2.Marriage != 0)
                    {
                        var Chrg = new ChrGet()
                        {
                            Entry = Chr2.Entry,
                        };
                        var Chrs = await ManageCharacter.GetAll(Chrg);
                        Chrs.Marriage = 0;
                        await ManageCharacter.UpdateAsync(Chrs);
                        await ctx.RespondAsync($"{Chr2.Name} divorced from {Chrs.Name}");
                    }
                    Chr.Marriage = Chr2.Entry;
                    Chr2.Marriage = Chr.Entry;
                    await ManageCharacter.UpdateAsync(Chr);
                    await ManageCharacter.UpdateAsync(Chr2);
                }
                else
                {
                    await ctx.RespondAsync($"I'm sorry- {Chr.Name}.. but {Chr2.Name} did not accept your proposal-");
                }
            }
            [GroupCommand]
            public async Task MainAsync(CommandContext ctx, [Description("Victi- i mean loved one."), RemainingText] string User)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                var Chr2 = await ManageCharacter.GetChrByName(User);
                var OwnKid = await ManageCharacter.GetOwnerByEntry(Chr2.Entry);
                var Victim = await ctx.Guild.GetMemberAsync(OwnKid.UID);
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"<@{Chr2.Name}>, Do you accept <@{Chr.Name}>'s request to marry?", 120000, ctx.Channel, Victim);
                if (r == null)
                {
                    await ctx.RespondAsync("Failed, no response found.");
                    return;
                }
                if (r.Content.ToLower().StartsWith("ye"))
                {
                    await ctx.RespondAsync($"{Chr.Name}, you may kiss your (Husband/Wife) {Chr2.Name}");
                    if (Chr.Marriage != 0)
                    {
                        var Chrg = new ChrGet()
                        {
                            Entry = Chr.Marriage,
                        };
                        var Chrs = await ManageCharacter.GetAll(Chrg);
                        Chrs.Marriage = 0;
                        await ManageCharacter.UpdateAsync(Chrs);
                        await ctx.RespondAsync($"{Chr.Name} divorced from {Chrs.Name}");
                    }
                    if (Chr2.Marriage != 0)
                    {
                        var Chrg = new ChrGet()
                        {
                            Entry = Chr2.Entry,
                        };
                        var Chrs = await ManageCharacter.GetAll(Chrg);
                        Chrs.Marriage = 0;
                        await ManageCharacter.UpdateAsync(Chrs);
                        await ctx.RespondAsync($"{Chr2.Name} divorced from {Chrs.Name}");
                    }
                    Chr.Marriage = Chr2.Entry;
                    Chr2.Marriage = Chr.Entry;
                    await ManageCharacter.UpdateAsync(Chr);
                    await ManageCharacter.UpdateAsync(Chr2);
                }
                else
                {
                    await ctx.RespondAsync($"I'm sorry- {Chr.Name}.. but {Chr2.Name} did not accept your proposal-");
                }
            }
            [Command("show")]
            public async Task MainshowAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Marriage == 0)
                {
                    await ctx.RespondAsync("You are not married");
                    return;
                }
                var Chrg = new ChrGet()
                {
                    Entry = Chr.Marriage,
                };
                var Chrs = await ManageCharacter.GetAll(Chrg);
                await ctx.RespondAsync($"{Chr.Name} is married to {Chrs.Name}");
            }
            [Command("divorce")]
            [Description("End your current marriage.")]
            public async Task DivorceAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Marriage != 0)
                {

                    var Chrg = new ChrGet()
                    {
                        Entry = Chr.Marriage,
                    };
                    var Chrs = await ManageCharacter.GetAll(Chrg);
                    Chrs.Marriage = 0;
                    await ManageCharacter.UpdateAsync(Chrs);
                    await ctx.RespondAsync($"{Chr.Name} divorced from {Chrs.Name}");
                    Chr.Marriage = 0;
                    await ManageCharacter.UpdateAsync(Chr);
                    return;
                }
                await ctx.RespondAsync("You aren't married.");
            }
            [Command("Procreate"), Aliases("Kid")]
            public async Task Kid(CommandContext ctx, [RemainingText] string Child)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Marriage != 0)
                {
                    var Kid = await ManageCharacter.GetChrByName(Child);
                    var OwnKid = await ManageCharacter.GetOwnerByEntry(Kid.Entry);
                    var Mem = await ctx.Guild.GetMemberAsync(OwnKid.UID);
                    var r = await Interactivity.WaitForAnswerAsync(ctx, $"<@{Kid.Name}>, Do you accept <@{Chr.Name}>'s request to be adopted?", 120000, ctx.Channel, Mem);
                    if (r == null)
                    {
                        await ctx.RespondAsync("Failed, no response found.");
                        return;
                    }
                    if (r.Content.ToLower() == "yes")
                    {
                        Chr.KID = Kid.Entry;
                        ChrGet = new ChrGet()
                        {
                            Entry = Chr.Marriage,
                        };
                        var Chr2 = await ManageCharacter.GetAll(ChrGet);
                        Chr2.KID = Kid.Entry;
                        await ManageCharacter.UpdateAsync(Chr);
                        await ManageCharacter.UpdateAsync(Chr2);
                        Kid.PARENTA = Chr.Entry;
                        Kid.PARENTB = Chr2.Entry;
                        await ManageCharacter.UpdateAsync(Kid);
                        await ctx.RespondAsync($"{Kid.Name} has been adopted!");
                    }
                    return;
                }
                await ctx.RespondAsync("You aren't married.");
            }
        }
        [Command("Remind")]
        public async Task Remind(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            if (Own.Remind == 0)
                Own.Remind = 1;
            else
                Own.Remind = 0;
            await ManageCharacter.UpdateAsync(Ownership: Own);
            await ctx.RespondAsync("(1:off/0:onn) Remind toggled " + Own.Remind);
        }
        [Command("List")]
        public async Task ListAsync(CommandContext ctx, string Genders)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            int i = 0;
            var sb = new StringBuilder();
            sb.Append("`Characters:`\n");
            int I = 0;
            while (i < 50)
            {
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = i
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                if (Own == null)
                {
                    i += 100;
                }
                else
                {
                    var ChrGet = new ChrGet()
                    {
                        Entry = Own.CharEntry,
                    };
                    var Chr = await ManageCharacter.GetAll(ChrGet);
                    if (Genders.ToLower() == "male")
                    {

                        if (Chr.Gender == Gender.Male)
                        {
                            sb.Append($"`[{i}]` **→** **[__{Chr.Class}__ - __{Chr.Name}__ - __{Chr.Race}__]**  [__Level:__ `{Chr.Level}`] **→** [__XP__ `{Chr.XP}|{Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level)}`]\n\n");
                            I += 1;
                        }
                    }
                    if (Genders.ToLower() == "female")
                    {
                        if (Chr.Gender == Gender.Female)
                        {
                            sb.Append($"`[{i}]` **→** **[__{Chr.Class}__ - __{Chr.Name}__ - __{Chr.Race}__]**  [__Level:__ `{Chr.Level}`] **→** [__XP__ `{Chr.XP}|{Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level)}`]\n\n");
                            I += 1;
                        }
                    }
                    i++;
                }
            }
            var embed = new DiscordEmbedBuilder();
            embed = embed.WithAuthor("You have " + I + " Characters of the category " + Genders + "!");
            var inter = ctx.Client.GetInteractivity();
            var pages = inter.GeneratePagesInEmbed(sb.ToString(), SplitType.Line, embed);
            await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
        [Command("List")]
        public async Task ListAsync(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            int i = 0;
            var sb = new StringBuilder();
            sb.Append("`Characters:`\n");
            while (i < 50)
            {
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = i
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                if (Own == null)
                {
                    i += 100;
                }
                else
                {
                    var ChrGet = new ChrGet()
                    {
                        Entry = Own.CharEntry,
                    };
                    var Chr = await ManageCharacter.GetAll(ChrGet);
                    sb.Append($"`[{i}]` **→** **[__{Chr.Class}__ - __{Chr.Name}__ - __{Chr.Race}__]**  [__Level:__ `{Chr.Level}`] **→** [__XP__ `{Chr.XP}|{Convert.ToInt32(Chr.Level * 100.57 / 4.2 * Chr.Level)}`]\n\n");
                    i++;
                }
            }
            var embed = new DiscordEmbedBuilder();
            var inter = ctx.Client.GetInteractivity();
            var pages = inter.GeneratePagesInEmbed(sb.ToString(), SplitType.Line, embed);
            await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
        [Command("Lvlup"), Aliases("up")]
        [Description("Spend your statpoints!")]
        public async Task LevelUpAsync(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);

            var emmy = await ManageCharacter.EmbedProfileAsync(Chr, ctx);
            var SPS = Chr.SP;
            if (SPS == 0)
                goto END;
            while (SPS > 0)
            {
                string stats = "`[HP]-[Energy]-[Sleight]-[Marksman]-[Dodge]-[Strength]-[Thought]-[Speed]-[intelligence]-[Magic]-[MagicEff] - [:save]`";
                string Q2 = $"Where do you want to spend your `{SPS}` Statpoints\n\n{stats}";
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ctx.Channel);
                var ch = ctx.Channel;
                string St = $"**How much do you want to spend in {r.Content.ToUpper()}?**";
                switch (r.Content.ToLower())
                {
                    case "hp":
                        var res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.HPM += res * 10;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "energy":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.ENM += res * 10;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "sleight":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < SPS + 1 && res > -1)
                        {
                            SPS -= res;
                            Chr.Sleight += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "marksman":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Marksman += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "dodge":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Dodge += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "strength":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Strenght += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "thought":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Thought += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "speed":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Speed += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "intelligence":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Intellegence += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "magic":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Magic += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "magiceff":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.MagicEff += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case ":save":
                        goto END;
                    default:
                        await r.RespondAsync("That is not one of our stats.");
                        break;
                }
            }
        END:
            Chr.SP = SPS;
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync("Done;");
        }
        [Command("SubClass")]
        public async Task SubClass(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            if (Chr.Level < 10 || Chr.SubClass != 0)
            {
                await ctx.RespondAsync("This is unavailable for you, you either have a subclass already or are too low level(10)");
                return;
            }
            switch (Chr.Class)
            {
                case Classes.Warrior:
                    await ctx.RespondAsync("choose from (Berserker, Crusader, Swordsman)");
                    break;
                case Classes.Archer:
                    await ctx.RespondAsync("choose from (Precision, Piercing, Balanced)");
                    break;
                case Classes.Defender:
                    await ctx.RespondAsync("choose from (Crusader, Tank)");
                    break;
                case Classes.Thief:
                    await ctx.RespondAsync("choose from (Stealth, Balanced)");
                    break;
                case Classes.Mage:
                    await ctx.RespondAsync("choose from (Priest, Wizard, Balanced)");
                    break;
            }
            string Q = $"What Subclass do you select?";
        ReAsk:
            var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ctx.Channel);

            switch (Chr.Class)
            {
                case Classes.Warrior:
                    switch (r.Content.ToLower())
                    {
                        case "berserker":
                            Chr.SubClass = SubClasses.Berserker;
                            Chr.Strenght += 5;
                            Chr.HPM += 20;
                            Chr.Sleight += 1;
                            break;
                        case "crusader":
                            Chr.SubClass = SubClasses.Crusader;
                            //Defender + Warrior Merged;
                            Chr.HPM += 60;
                            Chr.ENM += 20;
                            break;
                        case "swordsman":
                            Chr.SubClass = SubClasses.Swordsman;
                            Chr.Sleight += 6;
                            Chr.Dodge += 2;
                            break;
                        default:
                            await ctx.RespondAsync("Not one of the available subclasses, choose from (Berserker, Crusader, Swordsman)");
                            goto ReAsk;
                    }
                    break;
                case Classes.Archer:
                    switch (r.Content.ToLower())
                    {
                        case "precision":
                            Chr.SubClass = SubClasses.Precision;
                            Chr.Marksman += 8;
                            break;
                        case "piercing":
                            Chr.SubClass = SubClasses.Piercing;
                            Chr.Strenght += 8;
                            break;
                        case "balanced":
                            Chr.SubClass = SubClasses.Balanced;
                            Chr.Marksman += 4;
                            Chr.Strenght += 4;
                            break;
                        default:
                            await ctx.RespondAsync("Not one of the available subclasses, choose from (Precision, Piercing, Balanced)");
                            goto ReAsk;
                    }
                    break;
                case Classes.Defender:
                    switch (r.Content.ToLower())
                    {
                        case "crusader":
                            //Defender + Warrior Merged;
                            Chr.SubClass = SubClasses.Crusader;
                            Chr.HPM += 50;
                            Chr.Strenght += 3;
                            break;
                        case "tank":
                            Chr.SubClass = SubClasses.Tank;
                            Chr.HPM += 40;
                            Chr.Dodge += 4;
                            break;
                        default:
                            await ctx.RespondAsync("Not one of the available subclasses, choose from (Crusader, Tank)");
                            goto ReAsk;
                    }
                    break;
                case Classes.Thief:
                    switch (r.Content.ToLower())
                    {
                        case "stealth":
                            Chr.SubClass = SubClasses.Stealth;
                            Chr.Speed += 8;
                            break;
                        case "balanced":
                            Chr.SubClass = SubClasses.Balanced;
                            Chr.Dodge += 6;
                            Chr.Speed += 2;
                            break;
                        default:
                            await ctx.RespondAsync("Not one of the available subclasses, choose from (Stealth, Balanced)");
                            goto ReAsk;
                    }
                    break;
                case Classes.Mage:
                    switch (r.Content.ToLower())
                    {
                        case "priest":
                            Chr.SubClass = SubClasses.Priest;
                            Chr.SubClassAttribute += 15;
                            break;
                        case "wizard":
                            Chr.SubClass = SubClasses.Wizard;
                            Chr.MagicEff += 8;
                            break;
                        case "balanced":
                            Chr.SubClass = SubClasses.Balanced;
                            Chr.ENM += 40;
                            Chr.Magic += 2;
                            Chr.MagicEff += 2;
                            break;
                        default:
                            await ctx.RespondAsync("Not one of the available subclasses, choose from (Priest, Wizard, Balanced)");
                            goto ReAsk;
                    }
                    break;
            }
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync("Done;");

        }
        [Group("Unique")]
        public class Uniques : BaseCommandModule
        {
            [GroupCommand]
            public async Task UniqueAsync(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Level < 10)
                {
                    await ctx.RespondAsync("This is unavailable for you, you are too low level(10)");
                    return;
                }
                var Uniq = await ManageCharacter.GetAll(new UAbility()
                {
                    Character = Chr.Entry
                });
                if (Uniq == null)
                {
                    await ctx.RespondAsync("You don't have one..");
                    return;
                }
                var Emmy = new DiscordEmbedBuilder();
                Emmy = Emmy.WithAuthor($"{Chr.Name}'s Unique");
                Emmy = Emmy.WithDescription($"__**{Uniq.Name}**__\n\n`{Uniq.Description}`\n\n__type:__ `{Uniq.Path}`\n__Attribute:__ `{Uniq.Attribute}`");
                await ctx.RespondAsync(embed: Emmy);
            }
            [Command("upgrade"), Aliases("up")]
            public async Task UpThis(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Level < 10)
                {
                    await ctx.RespondAsync("This is unavailable for you, you are too low level(10)");
                    return;
                }
                var Uniq = await ManageCharacter.GetAll(new UAbility()
                {
                    Character = Chr.Entry
                });
                if (Uniq == null)
                {
                    await ctx.RespondAsync("You don't have one..");
                    return;
                }
                string Q = $"The next Upgrade costs ({Uniq.Attribute / 3}), which increases Unique Attribute by ({Uniq.Attribute / 4})\ncurrent: ({Uniq.Attribute})\nDo you want to upgade? (respond with yes)";

                if (Chr.UPoints < Uniq.Attribute / 3)
                {
                    await ctx.RespondAsync(Q);
                    await ctx.RespondAsync("Ending upgrade, you don't have that many points.");
                    return;
                }
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ctx.Channel);
                if (r.Content.ToLower() == "yes")
                    Uniq.Attribute += (Uniq.Attribute / 4);
                Chr.UPoints -= Uniq.Attribute / 2;
                await ManageCharacter.UpdateAsync(Chr);
                await ManageCharacter.UpdateAsync(Uniq: Uniq);
                await ctx.RespondAsync("Done;");
            }
            [Command("use")]
            public async Task Use(CommandContext ctx, DiscordUser Target = null)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, "&Unique Use");
                if (Cdr != null)
                {
                    await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                    return;
                }
                if (Chr.Level < 10)
                {
                    await ctx.RespondAsync("This is unavailable for you, you are too low level(10)");
                    return;
                }
                var OG = Chr.Name;
                var OGCHR = Chr;
                var Uniq = await ManageCharacter.GetAll(new UAbility()
                {
                    Character = Chr.Entry
                });
                if (Uniq == null)
                {
                    await ctx.RespondAsync("You don't have one..");
                    return;
                }
                switch (Uniq.Path)
                {
                    case UPath.boost:
                        Target = Target ?? ctx.User;
                        Accg = new AccountGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id
                        };
                        Check = await ManageCharacter.GetAll(Acc: Accg);
                        if (Check == null)
                        {
                            await ctx.RespondAsync("You have no characters.");
                            return;
                        }
                        OwnGet = new OwnerGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id,
                            Slot = Check.Slot
                        };
                        Own = await ManageCharacter.GetAll(OwnGet);
                        ChrGet = new ChrGet()
                        {
                            Entry = Own.CharEntry,
                        };
                        Chr = await ManageCharacter.GetAll(ChrGet);
                        if (Chr.EffectPath != 0)
                        {
                            await ctx.RespondAsync($"{Chr.Name} is already under an Boost or Weaken");
                            return;
                        }
                        Chr.EffectAtribute += Uniq.Attribute;
                        Chr.EffectPath = EffectPath.boost;
                        await ctx.RespondAsync($"{Chr.Name} has gained an Boost for 1 turn from {OG}");
                        await ManageCharacter.UpdateAsync(Chr);
                        break;
                    case UPath.damage:
                        if (Target == null)
                        {
                            await ctx.RespondAsync("You need to Mention a user.");
                            return;
                        }
                        Accg = new AccountGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id
                        };
                        Check = await ManageCharacter.GetAll(Acc: Accg);
                        if (Check == null)
                        {
                            await ctx.RespondAsync("You have no characters.");
                            return;
                        }
                        OwnGet = new OwnerGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id,
                            Slot = Check.Slot
                        };
                        Own = await ManageCharacter.GetAll(OwnGet);
                        ChrGet = new ChrGet()
                        {
                            Entry = Own.CharEntry,
                        };
                        Chr = await ManageCharacter.GetAll(ChrGet);
                        await Helpers.Cast.Damage(Convert.ToInt32(Uniq.Attribute * 2), Chr);
                        await ctx.RespondAsync($"{Chr.Name} takes {Convert.ToInt32(Uniq.Attribute * 2)} damage from {OG}!");
                        break;
                    case UPath.drain:
                        if (Target == null)
                        {
                            await ctx.RespondAsync("You need to Mention a user.");
                            return;
                        }
                        Accg = new AccountGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id
                        };
                        Check = await ManageCharacter.GetAll(Acc: Accg);
                        if (Check == null)
                        {
                            await ctx.RespondAsync("You have no characters.");
                            return;
                        }
                        OwnGet = new OwnerGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id,
                            Slot = Check.Slot
                        };
                        Own = await ManageCharacter.GetAll(OwnGet);
                        ChrGet = new ChrGet()
                        {
                            Entry = Own.CharEntry,
                        };
                        Chr = await ManageCharacter.GetAll(ChrGet);
                        await Helpers.Cast.Damage(Uniq.Attribute, Chr);
                        await Helpers.Cast.Heal(Uniq.Attribute / 2, OGCHR);
                        await ctx.RespondAsync($"{Chr.Name} takes {Uniq.Attribute} damage from {OG}, As it also healed {OG} for {Uniq.Attribute / 2}!");
                        break;
                    case UPath.heal:
                        Target = Target ?? ctx.User;
                        Accg = new AccountGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id
                        };
                        Check = await ManageCharacter.GetAll(Acc: Accg);
                        if (Check == null)
                        {
                            await ctx.RespondAsync("You have no characters.");
                            return;
                        }
                        OwnGet = new OwnerGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id,
                            Slot = Check.Slot
                        };
                        Own = await ManageCharacter.GetAll(OwnGet);
                        ChrGet = new ChrGet()
                        {
                            Entry = Own.CharEntry,
                        };
                        Chr = await ManageCharacter.GetAll(ChrGet);
                        await Helpers.Cast.Heal(Uniq.Attribute, Chr);
                        await ctx.RespondAsync($"{Chr.Name} gains {Uniq.Attribute} health from {OG}!");
                        break;
                    case UPath.weaken:
                        if (Target == null)
                        {
                            await ctx.RespondAsync("You need to Mention a user.");
                            return;
                        }
                        Accg = new AccountGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id
                        };
                        Check = await ManageCharacter.GetAll(Acc: Accg);
                        if (Check == null)
                        {
                            await ctx.RespondAsync("You have no characters.");
                            return;
                        }
                        OwnGet = new OwnerGet()
                        {
                            UID = ctx.User.Id,
                            GID = ctx.Guild.Id,
                            Slot = Check.Slot
                        };
                        Own = await ManageCharacter.GetAll(OwnGet);
                        ChrGet = new ChrGet()
                        {
                            Entry = Own.CharEntry,
                        };
                        Chr = await ManageCharacter.GetAll(ChrGet);
                        if (Chr.EffectPath != 0)
                        {
                            await ctx.RespondAsync($"{Chr.Name} is already under an Boost or Weaken");
                            return;
                        }
                        Chr.EffectAtribute += Uniq.Attribute;
                        Chr.EffectPath = EffectPath.weaken;
                        await ctx.RespondAsync($"{Chr.Name} has gained an Weakened effect for 1 turn from {OG}");
                        await ManageCharacter.UpdateAsync(Chr);
                        break;
                }
                await Helpers.TimerChr.AddCooldown(OGCHR.Entry, "&Unique Use", 1300);
            }
            [Command("use")]
            public async Task Use(CommandContext ctx, [RemainingText] string Target)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, "&Unique Use");
                if (Cdr != null)
                {
                    await ctx.RespondAsync($"This is on cooldown, please wait {(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds please!");
                    return;
                }
                if (Chr.Level < 10)
                {
                    await ctx.RespondAsync("This is unavailable for you, you are too low level(10)");
                    return;
                }
                var OG = Chr.Name;
                var OGCHR = Chr;
                var Uniq = await ManageCharacter.GetAll(new UAbility()
                {
                    Character = Chr.Entry
                });
                Chr = await ManageCharacter.GetChrByName(Target);
                if (Chr == null)
                {
                    await ctx.RespondAsync("No character by that name.");
                    return;
                }
                if (Uniq == null)
                {
                    await ctx.RespondAsync("You don't have one..");
                    return;
                }
                switch (Uniq.Path)
                {
                    case UPath.boost:
                        if (Chr.EffectPath != 0)
                        {
                            await ctx.RespondAsync($"{Chr.Name} is already under an Boost or Weaken");
                            return;
                        }
                        Chr.EffectAtribute += Uniq.Attribute;
                        Chr.EffectPath = EffectPath.boost;
                        await ctx.RespondAsync($"{Chr.Name} has gained an Boost for 1 turn from {OG}");
                        await ManageCharacter.UpdateAsync(Chr);
                        break;
                    case UPath.damage:
                        await Helpers.Cast.Damage(Convert.ToInt32(Uniq.Attribute * 2), Chr);
                        await ctx.RespondAsync($"{Chr.Name} takes {Convert.ToInt32(Uniq.Attribute * 2)} damage from {OG}!");
                        break;
                    case UPath.drain:
                        await Helpers.Cast.Damage(Uniq.Attribute, Chr);
                        await Helpers.Cast.Heal(Uniq.Attribute / 2, OGCHR);
                        await ctx.RespondAsync($"{Chr.Name} takes {Uniq.Attribute} damage from {OG}, As it also healed {OG} for {Uniq.Attribute / 2}!");
                        break;
                    case UPath.heal:
                        await Helpers.Cast.Heal(Uniq.Attribute, Chr);
                        await ctx.RespondAsync($"{Chr.Name} gains {Uniq.Attribute} health from {OG}!");
                        break;
                    case UPath.weaken:
                        if (Chr.EffectPath != 0)
                        {
                            await ctx.RespondAsync($"{Chr.Name} is already under an Boost or Weaken");
                            return;
                        }
                        Chr.EffectAtribute += Uniq.Attribute;
                        Chr.EffectPath = EffectPath.weaken;
                        await ctx.RespondAsync($"{Chr.Name} has gained an Weakened effect for 1 turn from {OG}");
                        await ManageCharacter.UpdateAsync(Chr);
                        break;
                }
                await Helpers.TimerChr.AddCooldown(OGCHR.Entry, "&Unique Use", 1300);
            }
            [Command("setup")]
            public async Task Create(CommandContext ctx)
            {
                var Accg = new AccountGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("You have no characters.");
                    return;
                }
                var OwnGet = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id,
                    Slot = Check.Slot
                };
                var Own = await ManageCharacter.GetAll(OwnGet);
                var ChrGet = new ChrGet()
                {
                    Entry = Own.CharEntry,
                };
                var Chr = await ManageCharacter.GetAll(ChrGet);
                if (Chr.Level < 10)
                {
                    await ctx.RespondAsync("This is unavailable for you, you are too low level(10)");
                    return;
                }
                var Uniq = await ManageCharacter.GetAll(new UAbility()
                {
                    Character = Chr.Entry
                });
                var UniqNew = new UAbility();
                UniqNew.Character = Chr.Entry;
                UniqNew.Attribute = 15;
                string Q = $"What Path of Ability? (Heal, Damage, Drain, Weaken, Boost)";
            Path:
                //None, damage, heal, drain, weaken, boost
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ctx.Channel);
                if (r == null)
                {
                    await ctx.RespondAsync("Timed out;");
                    return;
                }
                switch (r.Content.ToLower())
                {
                    case "damage":
                        UniqNew.Path = UPath.damage;
                        break;
                    case "heal":
                        UniqNew.Path = UPath.heal;
                        break;
                    case "drain":
                        UniqNew.Path = UPath.drain;
                        break;
                    case "weaken":
                        UniqNew.Path = UPath.weaken;
                        break;
                    case "boost":
                        UniqNew.Path = UPath.boost;
                        break;
                    default:
                        await ctx.RespondAsync("Not one of the paths.");
                        goto Path;
                }
                Q = "What name do you give this ability?";
            name:
                r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ctx.Channel);
                if (r == null)
                {
                    await ctx.RespondAsync("Timed out;");
                    return;
                }
                UniqNew.Name = r.Content;
                Q = "What Description do you give this ability?";
            desc:
                r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ctx.Channel);
                if (r == null)
                {
                    await ctx.RespondAsync("Timed out;");
                    return;
                }
                UniqNew.Description = r.Content;
                if (Uniq == null)
                {
                    await ManageCharacter.InsertAsync(Uniq: UniqNew);
                }
                else
                {
                    await ManageCharacter.UpdateAsync(Uniq: UniqNew);
                }
                await ctx.RespondAsync("Done;");
            }
        }

        [Command("Edit")]
        [Description("Edit your profile data(not everything is possible to be edited.)")]
        public async Task EditAsync(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync("You have no characters.");
                return;
            }
            var OwnGet = new OwnerGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);

            var emmy = await ManageCharacter.EmbedProfileAsync(Chr, ctx);
            for (; ; )
            {
                string stats = "`[Name]-[Age]-[Desc]-[Image]-[Theme]-[:save]`";
                string Q2 = $"What do you want to edit?\n\n{stats}";
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ctx.Channel);
                var ch = ctx.Channel;
                async Task suicide(string why = "")
                {
                    await ch.SendMessageAsync($"{why ?? ""} Stopping Edit.");
                };
                string St = $"**Enter a value for this field.**";
                switch (r.Content.ToLower())
                {
                    case "name":
                        var res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                        if (await ManageCharacter.CheckName(res.Content))
                        {
                            Chr.Name = res.Content;
                        }
                        else
                        {
                            await res.RespondAsync("That name is already used by someone else!");
                        }
                        break;
                    case "age":
                        var rs = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        Chr.Age = rs;
                        break;
                    case "desc":
                        res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                        Chr.Desc = res.Content;
                        break;
                    case "image":
                        res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                        r = res;
                        if (string.IsNullOrEmpty(r.Content))
                        {
                            if (r.Attachments.Count == 0)
                            {
                                await suicide("No image given.");
                                return;
                            }
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(r.Attachments.FirstOrDefault().Url, "./image.png");
                                DiscordChannel ccs = await ctx.Client.GetChannelAsync(705752802806202427);
                                DiscordMessage ms = await ccs.SendFileAsync("./image.png", ctx.User.Username, false, null, null);
                                Chr.Image = ms.Attachments.FirstOrDefault().Url;
                            }
                        }
                        else if (r.Content.ToLowerInvariant() != "none")
                        {
                            if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
                              && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                            {
                                Chr.Image = r.Content;
                            }
                            else
                            {
                                await suicide("Invalid Uri.");
                                return;
                            }
                        }
                        else
                        {
                            Chr.Image = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
                        }
                        break;
                    case "theme":
                        res = await Interactivity.WaitForAnswerAsync(ctx, St, channel: ch);
                        Chr.Theme = res.Content;
                        break;
                    case ":save":
                        goto END;
                    default:
                        await r.RespondAsync("That is not one of our fields.");
                        break;
                }
            }
        END:
            await ManageCharacter.UpdateAsync(Chr);
            await ctx.RespondAsync("Done;");
        }
        [Command("new")]
        [Description("Create a new Character.")]
        public async Task NewAsync(CommandContext ctx)
        {
            var Accg = new AccountGet()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id
            };
            var Check = await ManageCharacter.GetAll(Acc: Accg);
            var Acc = new Account();
            if (Check == null)
            {
                Acc.Slot = 0;
                Acc.GID = ctx.Guild.Id;
                Acc.UID = ctx.User.Id;
                await ManageCharacter.InsertAsync(Account: Acc);
            }
            else
            {
                Acc = Check;
                var Own = new OwnerGet()
                {
                    UID = ctx.User.Id,
                    GID = ctx.Guild.Id
                };
                int slot = await ManageCharacter.GetCountAsync(Tables.Ownership, Owner: Own);
                if (slot > 49)
                {
                    await ctx.RespondAsync("You already have 50 Characters.");
                    return;
                }
                Acc.Slot = slot;
                await ManageCharacter.UpdateAsync(Account: Acc);
            }
            var overwrites = new DiscordOverwriteBuilder[] {
            new DiscordOverwriteBuilder().Allow(Permissions.AccessChannels).For(ctx.Member),
            new DiscordOverwriteBuilder().Deny(Permissions.AccessChannels).For(ctx.Guild.EveryoneRole)
          };

            var ch = await ctx.Guild.CreateTextChannelAsync($"{ctx.User.Username}{ctx.User.Discriminator}-setup",
                overwrites: overwrites);
            await ch.SendMessageAsync($"{ctx.User.Mention}, here you will be creating an new character.\n\nRaces: Human, Cyborg, Dragonoid, Fairy, Kitsune, Elf, Spirit, Ghost, Celeste, Neko, Shapeshifter.\nClasses: Archer, Warrior, Mage, Thief, Defender");
            async Task suicide(string why = "")
            {
                await ch.SendMessageAsync($"{why ?? ""} Stopping setup.");
                await ch.SendMessageAsync("This channel shall kermit sewer side in 10 seconds.");
                await Task.Delay(10 * 1000);
                await ch.DeleteAsync();
            };
            int Question = 8;
            /*@Entry, @Name, @Gender, @Race, @Age, @Desc, @Image,
              @HPC, @HPM, @ENC, @ENM, @Class, @Sleight, @Marksman, @Dodge, @Strength,
              @Thought, @Speed, @Intellegence, @Magic, @MagicEff, @Level, @XP, @InventoryID, @SP*/
            var Chr = new Chr()
            {
                Name = "null",
                Age = 0,
                Gender = Gender.Male,
                Race = Race.Human,
                Desc = "null",
                Image = "null",

                Entry = 0,
                InventoryID = 0,

                HPC = 100,
                HPM = 100,
                ENC = 100,
                ENM = 100,

                Class = Classes.Warrior,

                Sleight = 0,
                Speed = 0,
                Marksman = 0,
                Dodge = 0,
                Strenght = 0,
                Thought = 0,
                Intellegence = 0,

                Magic = 0,
                MagicEff = 0,

                SP = 0,

                Level = 1,
                XP = 0,

                IsAlter = IsAlter.No,
                Balance = 100,

                Marriage = 0,

                SubClass = 0,
                SubClassAttribute = 0,
            };
            var Questions = new string[] { "Image", "Description", "State, Alter or not? (answer with YES or NO)", "Race", "Class", "Gender", "Age", "Name" };
            while (Question > 0)
            {
            QUESTION:
                int Ques = Question - 1;
                string Q = $"Give us your {Questions[Ques]}.";
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**`{Q}`**", channel: ch);
                if (r == null)
                {
                    await suicide();
                    return;
                }

                switch (Questions[Ques])
                {
                    case "Name":
                        if (await ManageCharacter.CheckName(r.Content))
                        {
                            Chr.Name = r.Content;
                            Question -= 1;
                        }
                        else
                        {
                            await r.RespondAsync("That name is already in use!");
                        }
                        break;
                    case "Age":
                        if (int.TryParse(r.Content, out var res))
                        {
                            Chr.Age = res;
                        }
                        else
                        {
                            Chr.Age = 0;
                        }
                        Question -= 1;
                        break;
                    case "Gender":
                        switch (r.Content.ToLower())
                        {
                            case "male":
                                Chr.Gender = Gender.Male;
                                break;
                            case "female":
                                Chr.Gender = Gender.Female;
                                break;
                            default:
                                var c = _rng.Next(1);
                                if (c == 0)
                                    Chr.Gender = Gender.Female;
                                if (c == 1)
                                    Chr.Gender = Gender.Male;
                                break;
                        }
                        Question -= 1;
                        break;
                    case "Race":
                        switch (r.Content.ToLower())
                        {
                            case "human":
                                Chr.Race = Race.Human;
                                Chr.Intellegence += 4;
                                Chr.Marksman += 2;
                                Chr.Strenght += 2;
                                break;
                            case "ghost":
                                Chr.Race = Race.Ghost;
                                Chr.MagicEff += 5;
                                Chr.Speed += 3;
                                Chr.Magic += 1;
                                break;
                            case "neko":
                                Chr.Race = Race.Neko;
                                Chr.Speed += 5;
                                Chr.Magic += 2;
                                Chr.MagicEff += 2;
                                break;
                            case "spirit":
                                Chr.Race = Race.Spirit;
                                Chr.Magic += 4;
                                Chr.MagicEff += 3;
                                Chr.ENM += 20;
                                break;
                            case "fairy":
                                Chr.Race = Race.Fairy;
                                Chr.Magic += 4;
                                Chr.MagicEff += 4;
                                Chr.HPM -= 10;
                                Chr.Thought += 2;
                                break;
                            case "elf":
                                Chr.Race = Race.Elf;
                                Chr.Magic += 4;
                                Chr.MagicEff += 4;
                                Chr.HPM -= 10;
                                Chr.Intellegence += 2;
                                break;
                            case "cyborg":
                                Chr.Race = Race.Cyborg;
                                Chr.HPM += 20;
                                Chr.Intellegence += 4;
                                Chr.Strenght += 3;
                                break;
                            case "celeste":
                                Chr.Race = Race.Celeste;
                                Chr.HPM += 60;
                                Chr.Dodge += 3;
                                break;
                            case "kitsune":
                                Chr.Race = Race.Kitsune;
                                Chr.Intellegence += 3;
                                Chr.Magic += 3;
                                Chr.MagicEff += 3;
                                break;
                            case "dragonoid":
                                Chr.Race = Race.Dragonoid;
                                Chr.Strenght += 3;
                                Chr.Magic += 1;
                                Chr.Marksman += 3;
                                Chr.Intellegence += 1;
                                Chr.Thought += 1;
                                break;
                            case "shapeshifter":
                                Chr.Race = Race.Shapeshifter;
                                Chr.Magic += 3;
                                Chr.Intellegence += 3;
                                Chr.Thought += 3;
                                break;
                            default:
                                await ch.SendMessageAsync("Invalid race");
                                goto QUESTION;
                        }
                        Question -= 1;
                        break;
                    case "Class":
                        switch (r.Content.ToLower())
                        {
                            case "warrior":
                                Chr.Class = Classes.Warrior;
                                Chr.Sleight += 6;
                                break;
                            case "archer":
                                Chr.Class = Classes.Archer;
                                Chr.Marksman += 6;
                                break;
                            case "mage":
                                Chr.Class = Classes.Mage;
                                Chr.MagicEff += 6;
                                break;
                            case "thief":
                                Chr.Class = Classes.Thief;
                                Chr.Dodge += 6;
                                break;
                            case "defender":
                                Chr.Class = Classes.Defender;
                                Chr.HPM += 30;
                                Chr.ENM += 30;
                                break;
                            default:
                                await ch.SendMessageAsync("Invalid Class");
                                goto QUESTION;
                        }
                        Question -= 1;
                        break;
                    case "Description":
                        Chr.Desc = r.Content;
                        Question -= 1;
                        break;
                    case "State, Alter or not? (answer with YES or NO)":
                        if (r.Content.StartsWith("ye"))
                            Chr.IsAlter = IsAlter.Yes;
                        Question -= 1;
                        break;
                    case "Image":
                        if (string.IsNullOrEmpty(r.Content))
                        {
                            if (r.Attachments.Count == 0)
                            {
                                await suicide("No image given.");
                                return;
                            }
                            using (WebClient webClient = new WebClient())
                            {
                                webClient.DownloadFile(r.Attachments.FirstOrDefault().Url, "./image.png");
                                DiscordChannel ccs = await ctx.Client.GetChannelAsync(705752802806202427);
                                DiscordMessage ms = await ccs.SendFileAsync("./image.png", ctx.User.Username, false, null, null);
                                Chr.Image = ms.Attachments.FirstOrDefault().Url;
                            }
                        }
                        else if (r.Content.ToLowerInvariant() != "none")
                        {
                            if (Uri.TryCreate(r.Content, UriKind.Absolute, out var uri)
                              && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                            {
                                Chr.Image = r.Content;
                            }
                            else
                            {
                                await suicide("Invalid Uri.");
                                return;
                            }
                        }
                        else
                        {
                            Chr.Image = "https://cdn.discordapp.com/attachments/704779039474319450/705474712607785051/7whzrBzDd7U2Tey7UAAAAAElFTkSuQmCC.png";
                        }
                        Question -= 1;
                        break;
                }

            }
            int SPS = 5;

            while (SPS > 0)
            {
                string stats = "`[HP]-[Energy]-[Sleight]-[Marksman]-[Dodge]-[Strength]-[Thought]-[Speed]-[Intelligence]-[Magic]-[MagicEff]`";
                string Q2 = $"Where do you want to spend your `{SPS}` Statpoints\n\n{stats}";
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**{Q2}**", channel: ch);
                string St = $"**How much do you want to spend in {r.Content.ToUpper()}?**";
                switch (r.Content.ToLower())
                {
                    case "hp":
                        var res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.HPM += res * 10;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "energy":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.ENM += res * 10;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "sleight":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < SPS + 1 && res > -1)
                        {
                            SPS -= res;
                            Chr.Sleight += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "marksman":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Marksman += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "dodge":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Dodge += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "strength":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Strenght += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "thought":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Thought += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "speed":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Speed += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "intelligence":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Intellegence += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "magic":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.Magic += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    case "magiceff":
                        res = await Interactivity.WaitForAnswerINTAsync(ctx, St, channel: ch);
                        if (res < (SPS + 1) && res > -1)
                        {
                            SPS -= res;
                            Chr.MagicEff += res;
                        }
                        else
                        {
                            await r.RespondAsync("You don't have that much SP");
                        }
                        break;
                    default:
                        await r.RespondAsync("That is not one of our stats.");
                        break;
                }
            }
            int result = await ManageCharacter.GetCountAsync(Tables.Character);
            Chr.InventoryID = result;
            Chr.Entry = result;


            var Ownr = new Ownership()
            {
                UID = ctx.User.Id,
                GID = ctx.Guild.Id,
                CharEntry = result,
                Slot = Acc.Slot
            };

            await ManageCharacter.InsertAsync(Chr, Ownership: Ownr);
            await ctx.RespondAsync("Done;");
            await suicide("You've finished setup, deleting channel soon.");
        }
    }
}
