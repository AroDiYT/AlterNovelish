using System;
using System.Threading.Tasks;
using System.Linq;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;

using System.Diagnostics;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules
{
    [Group("Currency"), Aliases(new string[] { "c", "cur" })]
    public class Currency : BaseCommandModule
    {
        [GroupCommand]
        [Description("Shows your or others current balance.")]
        public async Task ViewAsync(CommandContext ctx, DiscordUser Target = null)
        {
            Target = Target ?? ctx.User;
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
            var OwnGet = new OwnerGet()
            {
                UID = Target.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithAuthor($"{Chr.Name}");
            Embed = Embed.WithThumbnailUrl(Chr.Image);
            Embed = Embed.WithDescription($"{Chr.Balance} Enzea");
            await ctx.RespondAsync(embed: Embed);
        }
        [Command("work"), Aliases("w")]
        [Description("Gain a small amount of money by doing simple jobs.")]
        public async Task WorkAsync(CommandContext ctx)
        {
            var Target = ctx.User;
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
            var OwnGet = new OwnerGet()
            {
                UID = Target.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };
            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGet = new ChrGet()
            {
                Entry = Own.CharEntry,
            };
            var Chr = await ManageCharacter.GetAll(ChrGet);
            var Cdr = await Helpers.TimerChr.GetCooldown(Chr.Entry, "work");
            if(Cdr != null)
            {
              await ctx.RespondAsync($"You have already worked, wait for {(Cdr.RemainingTime/60).ToString() + " m " + (Cdr.RemainingTime - Convert.ToInt32(Cdr.RemainingTime/60)*60)} seconds please!");
              return;
            }

            var Amount = Helpers.Tools.RNG.Next(300);
            Chr.Balance += Amount;
            if (Amount < 1)
            {
                await ctx.RespondAsync("Sadly there where no jobs available- try again next time!");
            }
            else if (Amount > 125)
            {
                var Embed = new DiscordEmbedBuilder();
                Embed = Embed.WithAuthor($"{Chr.Name} worked hard and got a magnificent amount of money!");
                Embed = Embed.WithDescription($"{Amount} Enzea");
                await ctx.RespondAsync(embed: Embed);
            }
            else
            {
                var Embed = new DiscordEmbedBuilder();
                Embed = Embed.WithAuthor($"{Chr.Name} worked and got some money!");
                Embed = Embed.WithDescription($"{Amount} Enzea");
                await ctx.RespondAsync(embed: Embed);
            }
            await ManageCharacter.UpdateAsync(Chr);
            await Helpers.TimerChr.AddCooldown(Chr.Entry, "work", 120);
        }
    }
}
