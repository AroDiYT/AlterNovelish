using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System.Data;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using System.Diagnostics;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules
{
    public class MiscModule : BaseCommandModule
    {
        [Command("ping")]
        [Description("Showing information about the bot.")]
        public async Task PingAsync(CommandContext ctx)
        {
            var sw = Stopwatch.StartNew();
            var message = await ctx.RespondAsync("pinging"); // this sends a message
            sw.Stop();

            await message.ModifyAsync($"ping: {sw.ElapsedMilliseconds} ms\n\n" + $"Characters: {await ManageCharacter.GetCountAsync(Tables.Character)}"); // edit the message
        }
        [Command("Count")]
        public async Task Count(CommandContext ctx, [RemainingText] string text)
        {
            var Amount = text.Length;
            await ctx.RespondAsync("This text has " + Amount + " characters.");
        }
        [Command("calc")]
        public async Task Calculator(CommandContext ctx, [RemainingText] string Formula)
        {
            double result = Convert.ToDouble(new DataTable().Compute(Formula, null));
            await ctx.RespondAsync(result.ToString());
        }
        [Command("SaveAndQuit"), Aliases("SAQ")]
        [RequirePermissions(Permissions.Administrator)]
        public async Task SaveAndQuit(CommandContext ctx)
        {
            await ctx.RespondAsync("Bot is shutting down.");
            await Helpers.TimerChr.SaveCooldown();
            System.Environment.Exit(1);
        }
        [Command("Cooldowns"), Aliases("cdr")]
        public async Task Getcooldowns(CommandContext ctx)
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
            string Cdrs = "**Cooldowns:**";
            foreach (var ids in Helpers.TimerChr.Cooldowns.Keys)
            { // Only gets (int ID, string CMD)
                if (Chr.Entry == ids.ID)
                {
                    Cdrs += $"\n`{ids.CMD}`: ";
                    var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, ids.CMD);
                    Cdrs += $"{(Cdr.RemainingTime / 60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime / 60) * 60)} seconds";
                }
            }
            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithAuthor(Chr.Name, null, Chr.Image);
            Embed = Embed.WithDescription(Cdrs);

            await ctx.RespondAsync(embed: Embed);
        }
        [Command("chaos")]
        [Cooldown(1, 10, CooldownBucketType.Global)]
        public async Task ChaosAsync(CommandContext ctx, [RemainingText, Description("Victim")] string Target)
        {
            var chaos = ctx.Guild.Members.FirstOrDefault(kv => kv.Value.Username == "Chaosbringer")
            .Value;
            var emmy = new DiscordEmbedBuilder();
            var Text = "Is in Quene's house";
            if(!string.IsNullOrEmpty(Target))
            Text = "Wants to murder " + Target;
            var num = Helpers.Tools.RNG.Next(7) + 1;
            switch (num)
            {
                case 1:
                    emmy = emmy.WithDescription(chaos.Mention + " is a cute little cloud of chaos <3");
                    break;
                case 2:
                    emmy = emmy.WithDescription($"{chaos.Mention}, You are a nice little Chaos");
                    break;
                case 3:
                    emmy = emmy.WithDescription($"{chaos.Mention}, Dash says you are the best");
                    break;
                case 4:
                    emmy = emmy.WithDescription($"{chaos.Mention}  {Text}");
                    break;
                case 5:
                    emmy = emmy.WithDescription("THIS IS AN S.O.S. FROM DASH");
                    break;
                case 6:
                    emmy = emmy.WithDescription("You have escaped a ping but you are still a nice little Chaos");
                    break;
                case 7:
                    emmy = emmy.WithDescription($"{chaos.Mention} You are being summoned.");
                    break;
            }
            await ctx.RespondAsync(embed : emmy);
        }
        [Command("summon")]
        [Description("Summon a random animal.")]
        public async Task AnimalAsync(CommandContext ctx)
        {
            var ani = Helpers.Tools.RNG.Next(13) + 1;
            switch (ani)
            {
                case 1:
                    await ctx.RespondAsync(":service_dog:");
                    break;
                case 2:
                    await ctx.RespondAsync(":cat2:");
                    break;
                case 3:
                    await ctx.RespondAsync(":rabbit2:");
                    break;
                case 4:
                    await ctx.RespondAsync(":mouse2:");
                    break;
                case 5:
                    await ctx.RespondAsync(":peacock:");
                    break;
                case 6:
                    await ctx.RespondAsync(":sheep:");
                    break;
                case 7:
                    await ctx.RespondAsync(":unicorn:");
                    break;
                case 8:
                    await ctx.RespondAsync(":owl:");
                    break;
                case 9:
                    await ctx.RespondAsync(":pig2:");
                    break;
                case 10:
                    await ctx.RespondAsync(":giraffe:");
                    break;
                case 11:
                    await ctx.RespondAsync(":rooster:");
                    break;
                case 12:
                    await ctx.RespondAsync(":chipmunk:");
                    break;
                case 13:
                    await ctx.RespondAsync(":butterfly:");
                    break;
            }
        }
        [Command("Who")]
        [Description("Get a random person's mention.")]
        public async Task WhoAsync(CommandContext ctx)
        {
            var Emmy = new DiscordEmbedBuilder();
            Emmy = Emmy.WithDescription($"{ctx.Guild.Members.Values.ToArray()[Helpers.Tools.RNG.Next(ctx.Guild.Members.Count)].Mention}");
            await ctx.RespondAsync(embed: Emmy);
        }
        [Group("Snap")]
        public class Snaps : BaseCommandModule
        {
            [GroupCommand]
            public async Task SnapCount(CommandContext ctx, DiscordUser User = null)
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
                    await ctx.RespondAsync($"`{User.Username}` has no characters or an account.");
                    return;
                }
                await ctx.RespondAsync($"`{User.Username}` has " + Check.Snaps + " <:snap:713788866687139913>'s");
            }
            [Command("Add")]
            [RequirePermissions(Permissions.ManageMessages)]
            public async Task AddSnap(CommandContext ctx, DiscordUser user)
            {
                var Accg = new AccountGet()
                {
                    UID = user.Id,
                    GID = ctx.Guild.Id
                };
                var Check = await ManageCharacter.GetAll(Acc: Accg);
                if (Check == null)
                {
                    await ctx.RespondAsync("That person doesn't have an Account here.");
                    return;
                }
                Check.Snaps += 1;
                await ManageCharacter.UpdateAsync(Account: Check);
                await ctx.RespondAsync($"`{user.Username}` has broken the bot, a <:snap:713788866687139913> has been added to their account.");
            }
        }
        [Group("Issue")]
        public class Issues : BaseCommandModule
        {
            [GroupCommand]
            public async Task View(CommandContext ctx, int ID)
            {
                var count = await ManageCharacter.GetCountAsync(Tables.Issues);
                if (ID > count)
                {
                    await ctx.RespondAsync("I don't have that many errors asshole-");
                    return;
                }
                var Issue = await ManageCharacter.GetAll(new ManageCharacter.Issues()
                {
                    Entry = ID
                });
                if (Issue == null)
                {
                    await ctx.RespondAsync("ID not found...");
                    return;
                }
                string Solved = "Unsolved";
                if (Issue.Solved == ManageCharacter.Solved.yes)
                    Solved = "Solved";
                var Emmy = new DiscordEmbedBuilder()
                {
                    Description = $"`{Issue.Title}`\n\n{Issue.Desc}"
                };
                Emmy = Emmy.WithFooter("Issue #" + ID + $" [{Solved}]");
                await ctx.RespondAsync(embed: Emmy);
            }
            [GroupCommand]
            public async Task View(CommandContext ctx)
            {
                var count = await ManageCharacter.GetCountAsync(Tables.Issues);
                Console.WriteLine(count);
                string Text = "`Issues`";
                while (count > 0)
                {
                    var IssueGet = new ManageCharacter.Issues()
                    {
                        Entry = count - 1
                    };
                    var Issue = await ManageCharacter.GetAll(IssueGet);
                    if (Issue != null)
                    {
                        string Solved = "Unsolved";
                        if (Issue.Solved == ManageCharacter.Solved.yes)
                            Solved = "Solved";
                        Text += $"\n`→ [{Issue.Entry}] - {Issue.Title} - [{Solved}]`";
                    }
                    count--;
                }
                var embed = new DiscordEmbedBuilder();
                var inter = ctx.Client.GetInteractivity();
                var pages = inter.GeneratePagesInEmbed(Text, SplitType.Line, embed);
                await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
            }
            [Command("New")]
            public async Task New(CommandContext ctx)
            {
                var Issue = new ManageCharacter.Issues();
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**What is the title of this issue? (Command name, event name or just what it is)**", channel: ctx.Channel);
                if (r == null)
                    return;
                Issue.Title = r.Content;
                r = await Interactivity.WaitForAnswerAsync(ctx, $"**Describe the Issue and how it happened.**", channel: ctx.Channel);
                if (r == null)
                    return;
                Issue.Desc = r.Content;
                var count = await ManageCharacter.GetCountAsync(Tables.Issues);
                Issue.Entry = count;
                await ManageCharacter.InsertAsync(Issue: Issue);
                await ctx.RespondAsync("New Issue added, see it with &issue `" + count + "`");
            }
            [Command("Solved"), Aliases("Remove")]
            [RequirePermissions(Permissions.ManageMessages)]
            public async Task Solve(CommandContext ctx, int ID)
            {
                var count = await ManageCharacter.GetCountAsync(Tables.Issues);
                if (ID > count)
                {
                    await ctx.RespondAsync("I don't have that many errors asshole-");
                    return;
                }
                var Issue = await ManageCharacter.GetAll(new ManageCharacter.Issues()
                {
                    Entry = ID
                });
                if (Issue == null)
                {
                    await ctx.RespondAsync("ID not found...");
                    return;
                }
                Issue.Solved = ManageCharacter.Solved.yes;
                var Emmy = new DiscordEmbedBuilder()
                {
                    Description = $"`[Solved]{Issue.Title}`\n\n{Issue.Desc}"
                };
                Emmy = Emmy.WithFooter("Issue #" + ID);
                await ctx.RespondAsync(embed: Emmy);
                await ManageCharacter.UpdateAsync(Issue: Issue);
                await Task.Delay(1000 * 120);
                await ManageCharacter.RemoveAsync(Issue);
            }
        }
        [Group("Suggest")]
        public class Suggestion : BaseCommandModule
        {
            [GroupCommand]
            public async Task View(CommandContext ctx, int ID)
            {
                var Issue = await ManageCharacter.GetAlls(new ManageCharacter.Suggestions()
                {
                    Entry = ID
                });
                if (Issue == null)
                {
                    await ctx.RespondAsync("ID not found...");
                    return;
                }
                string Solved = "Not Added";
                if (Issue.Done == ManageCharacter.Done.yes)
                    Solved = "Done";
                var Emmy = new DiscordEmbedBuilder()
                {
                    Description = $"`{Issue.Title}`\n\n{Issue.Desc}"
                };
                Emmy = Emmy.WithFooter("Issue #" + ID + $" [{Solved}]");
                await ctx.RespondAsync(embed: Emmy);
            }
            [GroupCommand]
            public async Task View(CommandContext ctx)
            {
                var count = await ManageCharacter.GetCountAsync(Tables.Suggest);
                Console.WriteLine(count);
                string Text = "`Suggestions`";
                while (count > 0)
                {
                    var IssueGet = new ManageCharacter.Suggestions()
                    {
                        Entry = count - 1
                    };
                    var Issue = await ManageCharacter.GetAlls(IssueGet);
                    if (Issue != null)
                    {
                        string Solved = "Not Added";
                        if (Issue.Done == ManageCharacter.Done.yes)
                            Solved = "Done";
                        Text += $"\n`→ [{Issue.Entry}] - {Issue.Title} - [{Solved}]`";
                    }
                    count--;
                }
                var embed = new DiscordEmbedBuilder();
                var inter = ctx.Client.GetInteractivity();
                var pages = inter.GeneratePagesInEmbed(Text, SplitType.Line, embed);
                await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
            }
            [Command("New")]
            public async Task New(CommandContext ctx)
            {
                var Issue = new ManageCharacter.Suggestions();
                var r = await Interactivity.WaitForAnswerAsync(ctx, $"**What is the title of this Suggestion? (Command name, event name or just what it is)**", channel: ctx.Channel);
                if (r == null)
                    return;
                Issue.Title = r.Content;
                r = await Interactivity.WaitForAnswerAsync(ctx, $"**Describe the Suggestion**", channel: ctx.Channel);
                if (r == null)
                    return;
                Issue.Desc = r.Content;
                var count = await ManageCharacter.GetCountAsync(Tables.Suggest);
                Issue.Entry = count;
                await ManageCharacter.InsertAsync(Suggest: Issue);
                await ctx.RespondAsync("New Suggestion added, see it with &Suggestion `" + count + "`");
            }
            [Command("Done"), Aliases("Remove")]
            [RequirePermissions(Permissions.ManageMessages)]
            public async Task Solve(CommandContext ctx, int ID)
            {
                var Issue = await ManageCharacter.GetAlls(new ManageCharacter.Suggestions()
                {
                    Entry = ID
                });
                if (Issue == null)
                {
                    await ctx.RespondAsync("ID not found...");
                    return;
                }
                Issue.Done = ManageCharacter.Done.yes;
                var Emmy = new DiscordEmbedBuilder()
                {
                    Description = $"`[Done]{Issue.Title}`\n\n{Issue.Desc}"
                };
                Emmy = Emmy.WithFooter("Suggestion #" + ID);
                await ctx.RespondAsync(embed: Emmy);
                await ManageCharacter.UpdateAsync(Suggest: Issue);
                await Task.Delay(1000 * 120);
                await ManageCharacter.RemoveAsync(Issue);
            }
        }

    }
}
