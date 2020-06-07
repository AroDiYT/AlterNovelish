using System;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;

using BotTemplate.Managers;
using BotTemplate.Objects.Sql.Profile;

namespace BotTemplate.Modules
{
    [Group("Card"), Aliases("cards")]
    [Description("Collect all the characters")]
    public class Cards : BaseCommandModule
    {
        [GroupCommand()]
        public async Task ViewSelf(CommandContext ctx)
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
                await ctx.RespondAsync($"You don't own a character.");
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
            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithDescription($"[`Card`] → [__{Chr.Class} - {Chr.Name} - {Chr.Race}__]");
            Embed = Embed.WithImageUrl(Chr.Image);
            Embed = Embed.WithFooter(Chr.Desc);
            await ctx.RespondAsync(embed: Embed);
        }
        [Command("search")]
        [Description("Seek a specific card.")]
        public async Task ViewOther(CommandContext ctx, [RemainingText] string Name)
        {
            var Chr = await ManageCharacter.GetChrByName(Name);
            if (Chr == null)
            {
                await ctx.RespondAsync("No character by " + Name);
                return;
            }
            var Own = await ManageCharacter.GetOwnerByEntry(Chr.Entry);
            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithDescription($"[`Card`] → [__{Chr.Class} - {Chr.Name} - {Chr.Race}__]");
            Embed = Embed.WithImageUrl(Chr.Image);
            Embed = Embed.WithFooter(Chr.Desc);
            await ctx.RespondAsync(embed: Embed);
        }
        [Command("Inventory"), Aliases("Inv")]
        public async Task InvCards(CommandContext ctx)
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
            var Chrs = await ManageCharacter.GetAll(ChrGet);
            int i = 0;
            var sb = new StringBuilder();
            sb.Append($"`{Chrs.Name}'s Cards:`");
            while (i < await ManageCharacter.GetCountAsync(Tables.Character))
            {
                var Card = new ManageCharacter.Card()
                {
                    ChrEntry = Chrs.Entry,
                    ChrCode = i
                };
                var Cards = await ManageCharacter.GetAll(Card);
                if (Cards == null)
                    i++;
                else
                {
                    var Chr = await ManageCharacter.GetAll(Card);
                    sb.Append($"`Amount[{Chr.ChrAmount}]` **→** **[__{Chr.ChrClass}__ - __{Chr.ChrName}__ - __{Chr.ChrRace}__]**\n");
                    i++;
                }
            }
            var embed = new DiscordEmbedBuilder();
            var inter = ctx.Client.GetInteractivity();
            var pages = inter.GeneratePagesInEmbed(sb.ToString(), SplitType.Line, embed);
            await inter.SendPaginatedMessageAsync(ctx.Channel, ctx.Member, pages);
        }
        [Command("Summon")]
        [Cooldown(3,2400,CooldownBucketType.User)]
        public async Task SummonCard(CommandContext ctx)
        {
            var CharEntry = Helpers.Tools.RNG.Next(await ManageCharacter.GetCountAsync(Tables.Character));
            var ChrGet = new ChrGet()
            {
                Entry = CharEntry,
            };

            var Chr = await ManageCharacter.GetAll(ChrGet);

            var Embed = new DiscordEmbedBuilder();
            Embed = Embed.WithDescription($"[`.Claim`] → [__{Chr.Class} - {Chr.Name} - {Chr.Race}__]");
            Embed = Embed.WithImageUrl(Chr.Image);
            Embed = Embed.WithFooter(Chr.Desc);
            await ctx.RespondAsync(embed: Embed);

        ReAsk:
            var r = await Interactivity.WaitForStealAsync(ctx, channel: ctx.Channel);
            if (r == null)
                goto ReAsk;

            var User = r;

            var Accg = new AccountGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id
            };

            var Check = await ManageCharacter.GetAll(Acc: Accg);
            if (Check == null)
            {
                await ctx.RespondAsync($"You don't own a character.");
                goto ReAsk;
            }

            var OwnGet = new OwnerGet()
            {
                UID = User.Id,
                GID = ctx.Guild.Id,
                Slot = Check.Slot
            };

            var Own = await ManageCharacter.GetAll(OwnGet);
            var ChrGets = new ChrGet()
            {
                Entry = Own.CharEntry,
            };

            var Chrs = await ManageCharacter.GetAll(ChrGets);
            var Card = new ManageCharacter.Card()
            {
                ChrCode = Chr.Entry,
                ChrEntry = Chrs.Entry,
                ChrName = Chr.Name,
                ChrDesc = Chr.Desc,
                ChrRace = Chr.Race,
                ChrClass = Chr.Class,
                ChrAmount = 1,
                ChrImage = Chr.Image
            };

            var Rs = await ManageCharacter.GetAll(Card);
            if (Rs == null)
            {
                await ctx.RespondAsync("You claimed this card for the first time!");
                await ManageCharacter.InsertAsync(Card: Card);
                return;
            }

            Card.ChrAmount += Rs.ChrAmount;

            await ManageCharacter.UpdateAsync(Card: Card);
            await ctx.RespondAsync("You claimed this card!");
        }
    }
}
